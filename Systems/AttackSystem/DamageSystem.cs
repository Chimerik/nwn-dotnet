using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using System;
using Action = System.Action;
using Context = NWN.Systems.Config.Context;

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
            ProcessDamageCalculations,
            ProcessTargetItemDurability,
      }
    );
    public static void HandleDamageEvent(OnCreatureDamage onDamage)
    {
      PlayerSystem.Log.Info("Entering Damage Event");

      if (onDamage.DamagedBy != null)
        PlayerSystem.Log.Info("DamagedBy : " + onDamage.DamagedBy.Name);

      if (onDamage.Target == null)
      {
        PlayerSystem.Log.Info("Damage target null");
        return;
      }

      if (onDamage.Target.GetObjectVariable<LocalVariableInt>($"_DAMAGE_HANDLED_FROM_{onDamage.DamagedBy}").HasValue)
      {
        onDamage.Target.GetObjectVariable<LocalVariableInt>($"_DAMAGE_HANDLED_FROM_{onDamage.DamagedBy}").Delete();
        return;
      }

      if (!(onDamage.Target is NwCreature oTarget))
        return;

      //await NwModule.Instance.WaitForObjectContext();

      damagePipeline.Execute(new Context(
        onAttack: null,
        oTarget: oTarget,
        onDamage: onDamage
      ));

      /*if (onDamage.Target.GetObjectVariable<LocalVariableInt>("_IS_GNOME_MECH").HasValue && onDamage.DamageData.Electrical > 0)
      {
        onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.Heal(onDamage.DamageData.Electrical));
        onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));
        onDamage.Target.GetObjectVariable<LocalVariableInt>("_SPARK_LEVEL").Value += 5;

        foreach (NwCreature oPC in onDamage.Target.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.ControllingPlayer != null))
          oPC.ControllingPlayer.DisplayFloatingTextStringOnCreature((NwCreature)onDamage.Target, onDamage.DamageData.Electrical.ToString().ColorString(new Color(32, 255, 32)));

        if (onDamage.Target.Tag == "dog_meca_defect" && CreaturePlugin.GetAttacksPerRound(onDamage.Target) < 6
          && NwRandom.Roll(Utils.random, 100) <= onDamage.Target.GetObjectVariable<LocalVariableInt>("_SPARK_LEVEL").Value)
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
      // TODO : voir comment le "damager" est détecté dans le cas des AoE FNF et des AoE qui restent plus longtemps au sol => il est null s'il est déco ou mort. Peut-être mettre le damage en variable locale sur l'AoE créée et le récupérer de là

      if (ctx.oAttacker != null && ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_SPELL_ATTACK_POSITION").HasValue)
        ctx.attackPosition = (Config.AttackPosition)ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_SPELL_ATTACK_POSITION").Value;
      else
        ctx.attackPosition = Config.AttackPosition.NormalOrRanged;

      next();
    }

    private static void ProcessMageStaffBonusDamage(Context ctx, Action next)
    {
      // TODO : Ajouter des dégâts bonus au sort si l'arme en main est un bâton de mage et qu'il dispose de propriétés de bonus de dégâts

      next();
    }
  }
}
