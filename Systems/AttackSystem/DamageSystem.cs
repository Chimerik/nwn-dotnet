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
            ProcessTargetSpellDamageAbsorption,
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
    private static void ProcessTargetSpellDamageAbsorption(Context ctx, Action next)
    {
      PlayerSystem.Log.Info($"ProcessTargetSpellDamageAbsorption");
      if (ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin) != null)
      {
        ItemProperty absorption = null;
        int bonusAbsorbedDamage = 0;

        if (ctx.onDamage.DamageData.Bludgeoning > 0)
        {
          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 0 || i.SubType == 4) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption != null)
          {
            switch (absorption.CostTableValue)
            {
              case 8:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Bludgeoning / 4;
                ctx.onDamage.DamageData.Bludgeoning -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
                break;
              case 9:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Bludgeoning / 2;
                ctx.onDamage.DamageData.Bludgeoning -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
                break;
              case 10:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Bludgeoning * 3 / 4;
                ctx.onDamage.DamageData.Bludgeoning -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
                break;
              case 11:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Bludgeoning;
                ctx.onDamage.DamageData.Bludgeoning = 0;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
                break;
            }
          }
        }

        if (ctx.onDamage.DamageData.Pierce > 0)
        {
          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 1 || i.SubType == 4) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption != null)
          {
            switch (absorption.CostTableValue)
            {
              case 8:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Pierce / 4;
                ctx.onDamage.DamageData.Pierce -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
                break;
              case 9:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Pierce / 2;
                ctx.onDamage.DamageData.Pierce -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
                break;
              case 10:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Pierce * 3 / 4;
                ctx.onDamage.DamageData.Pierce -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
                break;
              case 11:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Pierce;
                ctx.onDamage.DamageData.Pierce = 0;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
                break;
            }
          }
        }

        if (ctx.onDamage.DamageData.Slash > 0)
        {
          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 2 || i.SubType == 4) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption != null)
          {
            switch (absorption.CostTableValue)
            {
              case 8:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Slash / 4;
                ctx.onDamage.DamageData.Slash -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
                break;
              case 9:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Slash / 2;
                ctx.onDamage.DamageData.Slash -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
                break;
              case 10:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Slash * 3 / 4;
                ctx.onDamage.DamageData.Slash -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
                break;
              case 11:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Slash;
                ctx.onDamage.DamageData.Slash = 0;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
                break;
            }
          }
        }

        if (ctx.onDamage.DamageData.Electrical > 0)
        {
          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 9 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption != null)
          {
            switch (absorption.CostTableValue)
            {
              case 8:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Electrical / 4;
                ctx.onDamage.DamageData.Electrical -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
                break;
              case 9:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Electrical / 2;
                ctx.onDamage.DamageData.Electrical -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
                break;
              case 10:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Electrical * 3 / 4;
                ctx.onDamage.DamageData.Electrical -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
                break;
              case 11:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Electrical;
                ctx.onDamage.DamageData.Electrical = 0;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
                break;
            }
          }
        }

        if (ctx.onDamage.DamageData.Cold > 0)
        {
          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 7 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption != null)
          {
            switch (absorption.CostTableValue)
            {
              case 8:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Cold / 4;
                ctx.onDamage.DamageData.Cold -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
                break;
              case 9:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Cold / 2;
                ctx.onDamage.DamageData.Cold -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
                break;
              case 10:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Cold * 3 / 4;
                ctx.onDamage.DamageData.Cold -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
                break;
              case 11:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Cold;
                ctx.onDamage.DamageData.Cold = 0;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
                break;
            }
          }
        }

        if (ctx.onDamage.DamageData.Fire > 0)
        {
          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 10 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption != null)
          {
            switch (absorption.CostTableValue)
            {
              case 8:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Fire / 4;
                ctx.onDamage.DamageData.Fire -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
                break;
              case 9:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Fire / 2;
                ctx.onDamage.DamageData.Fire -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
                break;
              case 10:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Fire * 3 / 4;
                ctx.onDamage.DamageData.Fire -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
                break;
              case 11:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Fire;
                ctx.onDamage.DamageData.Fire = 0;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
                break;
            }
          }
        }

        if (ctx.onDamage.DamageData.Magical > 0)
        {
          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 5 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption != null)
          {
            switch (absorption.CostTableValue)
            {
              case 8:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Magical / 4;
                ctx.onDamage.DamageData.Magical -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
                break;
              case 9:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Magical / 2;
                ctx.onDamage.DamageData.Magical -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
                break;
              case 10:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Magical * 3 / 4;
                ctx.onDamage.DamageData.Magical -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
                break;
              case 11:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Magical;
                ctx.onDamage.DamageData.Magical = 0;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
                break;
            }
          }
        }

        if (ctx.onDamage.DamageData.Sonic > 0)
        {
          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 13 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption != null)
          {
            switch (absorption.CostTableValue)
            {
              case 8:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Sonic / 4;
                ctx.onDamage.DamageData.Sonic -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
                break;
              case 9:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Sonic / 2;
                ctx.onDamage.DamageData.Sonic -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
                break;
              case 10:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Sonic * 3 / 4;
                ctx.onDamage.DamageData.Sonic -= (short)bonusAbsorbedDamage;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
                break;
              case 11:
                bonusAbsorbedDamage = ctx.onDamage.DamageData.Sonic;
                ctx.onDamage.DamageData.Sonic = 0;
                HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
                break;
            }
          }
        }
      }

      next();
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
