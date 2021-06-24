using NWN.API;
using NWN.API.Constants;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API.Events;
using System.Linq;
using System;
using Action = System.Action;
using System.Collections.Generic;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static Pipeline<Context> damagePipeline = new Pipeline<Context>(
      new Action<Context, Action>[]
      {
            ProcessTargetDamageAbsorption,
            //ProcessBaseArmorSpellPenetration,
            //ProcessBonusArmorSpellPenetration,
            ProcessSpellAttackPosition,
            ProcessArmorSlotHit,
            ProcessTargetSpecificAC,
            ProcessTargetShieldAC,
            ProcessArmorPenetrationCalculations,
            ProcessSpellDamageCalculations,
            ProcessTargetItemDurability,
      }
    );
    public static async void HandleDamageEvent(OnCreatureDamage onDamage)
    {
      PlayerSystem.Log.Info("Entering Damage Event");

      if (onDamage.DamagedBy != null)
        PlayerSystem.Log.Info("DamagedBy : " + onDamage.DamagedBy.Name);

      if (onDamage.Target == null)
      {
        PlayerSystem.Log.Info("Damage target null");
        return;
      }

      if (onDamage.Target.GetLocalVariable<int>($"_DAMAGE_HANDLED_FROM_{onDamage.DamagedBy}").HasValue)
      {
        onDamage.Target.GetLocalVariable<int>($"_DAMAGE_HANDLED_FROM_{onDamage.DamagedBy}").Delete();
        return;
      }

      PlayerSystem.Log.Info("Base : " + onDamage.DamageData.Base);
      PlayerSystem.Log.Info("Blud : " + onDamage.DamageData.Bludgeoning);
      PlayerSystem.Log.Info("Pierce : " + onDamage.DamageData.Pierce);
      PlayerSystem.Log.Info("Slash : " + onDamage.DamageData.Slash);

      if (!(onDamage.Target is NwCreature oTarget))
        return;

      await NwModule.Instance.WaitForObjectContext();

      damagePipeline.Execute(new Context(
        onAttack: null,
        oTarget: oTarget,
        onDamage: onDamage
      ));

      /*if (onDamage.Target.GetLocalVariable<int>("_IS_GNOME_MECH").HasValue && onDamage.DamageData.Electrical > 0)
      {
        onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.Heal(onDamage.DamageData.Electrical));
        onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));
        onDamage.Target.GetLocalVariable<int>("_SPARK_LEVEL").Value += 5;

        foreach (NwCreature oPC in onDamage.Target.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.ControllingPlayer != null))
          oPC.ControllingPlayer.DisplayFloatingTextStringOnCreature((NwCreature)onDamage.Target, onDamage.DamageData.Electrical.ToString().ColorString(new Color(32, 255, 32)));

        if (onDamage.Target.Tag == "dog_meca_defect" && CreaturePlugin.GetAttacksPerRound(onDamage.Target) < 6
          && NwRandom.Roll(Utils.random, 100) <= onDamage.Target.GetLocalVariable<int>("_SPARK_LEVEL").Value)
        {
          foreach (NwCreature oPC in onDamage.Target.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.ControllingPlayer != null))
            oPC.ControllingPlayer.SendServerMessage("Attention, la décharge électrique surcharge le chien mécanisé défectueux, le rendant plus dangereux !");

          onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
          onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.Damage(onDamage.Target.MaxHP / 10, DamageType.Magical));
          ((NwCreature)onDamage.Target).BaseAttackCount = CreaturePlugin.GetAttacksPerRound(onDamage.Target) + 1;
        }
        
        onDamage.DamageData.Electrical = 0;
      }*/
    }

    private static void ProcessBaseArmorSpellPenetration(Context ctx, Action next)
    {
      // TODO : la pénétration dépendra du caster level, du sort lancé (les sorts de foudre ont 25 % de pénétration de base) et probablement d'autres trucs entraînés

      next();
    }

    private static void ProcessSpellAttackPosition(Context ctx, Action next)
    {
      PlayerSystem.Log.Info($"ProcessSpellAttackPosition");
      // TODO : voir comment le "damager" est détecté dans le cas des AoE FNF et des AoE qui restent plus longtemps au sol

      if (ctx.onDamage.DamagedBy != null)
      {

        if (ctx.onDamage.DamagedBy.GetLocalVariable<int>("_SPELL_ATTACK_POSITION").HasValue)
          ctx.attackPosition = (Config.AttackPosition)ctx.onDamage.DamagedBy.GetLocalVariable<int>("_SPELL_ATTACK_POSITION").Value;
      }
      next();
    }
    private static void ProcessSpellDamageCalculations(Context ctx, Action next)
    {
      PlayerSystem.Log.Info($"ProcessSpellDamageCalculations");
      if (ctx.onDamage.DamageData.Base > 0)
      {
        ctx.onDamage.DamageData.Base = (short)(ctx.onDamage.DamageData.Base * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Piercing) - 60) / 40));
      }

      if (ctx.onDamage.DamageData.Bludgeoning > 0)
        ctx.onDamage.DamageData.Bludgeoning = (short)(ctx.onDamage.DamageData.Bludgeoning * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Bludgeoning) - 60) / 40));

      if (ctx.onDamage.DamageData.Pierce > 0)
        ctx.onDamage.DamageData.Pierce = (short)(ctx.onDamage.DamageData.Pierce * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Piercing) - 60) / 40));

      if (ctx.onDamage.DamageData.Slash > 0)
        ctx.onDamage.DamageData.Slash = (short)(ctx.onDamage.DamageData.Slash * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Slashing) - 60) / 40));

      if (ctx.onDamage.DamageData.Electrical > 0)
        ctx.onDamage.DamageData.Electrical = (short)(ctx.onDamage.DamageData.Electrical * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Electrical) - 60) / 40));

      if (ctx.onDamage.DamageData.Acid > 0)
        ctx.onDamage.DamageData.Acid = (short)(ctx.onDamage.DamageData.Acid * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Acid) - 60) / 40));

      if (ctx.onDamage.DamageData.Cold > 0)
        ctx.onDamage.DamageData.Cold = (short)(ctx.onDamage.DamageData.Cold * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Cold) - 60) / 40));

      if (ctx.onDamage.DamageData.Fire > 0)
        ctx.onDamage.DamageData.Fire = (short)(ctx.onDamage.DamageData.Fire * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Fire) - 60) / 40));

      if (ctx.onDamage.DamageData.Magical > 0)
        ctx.onDamage.DamageData.Magical = (short)(ctx.onDamage.DamageData.Magical * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Magical) - 60) / 40));

      if (ctx.onDamage.DamageData.Sonic > 0)
        ctx.onDamage.DamageData.Sonic = (short)(ctx.onDamage.DamageData.Sonic * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Sonic) - 60) / 40));

      next();
    }
  }
}
