using NWN.API;
using NWN.API.Constants;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API.Events;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Action = System.Action;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public class Context
    {
      public OnCreatureAttack onAttack { get; set; }
      public int baseDamageType { get; set; }
      public NwCreature oTarget { get; }
      public bool isUnarmedAttack { get; }
      public bool isRangedAttack { get; }
      public NwItem attackWeapon { get; set; }
      public NwItem targetArmor { get; set; }
      public int baseArmorPenetration { get; set; }
      public int bonusArmorPenetration { get; set; }
      public Config.AttackPosition attackPosition { get; set; }
      public Dictionary<DamageType, int> targetAC { get; set; }

      public Context(OnCreatureAttack onAttack, NwCreature oTarget)
      {
        this.onAttack = onAttack;
        this.oTarget = oTarget;
        this.attackWeapon = null;
        this.targetArmor = null;
        this.baseDamageType = 3; // Slashing par défaut
        this.baseArmorPenetration = 0;
        this.bonusArmorPenetration = 0;
        this.attackPosition = Config.AttackPosition.NormalOrRanged;
        this.isUnarmedAttack = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand) == null;
        this.isRangedAttack = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand) != null
        && ItemUtils.GetItemCategory(onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand).BaseItemType) == ItemUtils.ItemCategory.RangedWeapon;
        this.targetAC = new Dictionary<DamageType, int>();
        targetAC.Add(DamageType.BaseWeapon, 0);
      }
    }

    public static Pipeline<Context> pipeline = new Pipeline<Context>(
      new Action<Context, Action>[]
      {
            IsAttackDodged,
            ProcessAutomaticHit,
            ProcessBaseDamageTypeAndAttackWeapon,
            ProcessTargetDamageAbsorption,
            ProcessBaseArmorPenetration,
            ProcessBonusArmorPenetration,
            ProcessAttackPosition,
            ProcessArmorSlotHit,
            ProcessTargetSpecificAC,
            ProcessTargetShieldAC,
            ProcessArmorPenetrationCalculations,
            ProcessDamageCalculations,
            ProcessAttackerItemDurability,
            ProcessTargetItemDurability,
      }
    );
    public static async void HandleAttackEvent(OnCreatureAttack onAttack)
    {
      PlayerSystem.Log.Info("Entering Attack Event");
      PlayerSystem.Log.Info("Base : " + onAttack.DamageData.Base);
      PlayerSystem.Log.Info("Blud : " + onAttack.DamageData.Bludgeoning);
      PlayerSystem.Log.Info("Pierce : " + onAttack.DamageData.Pierce);
      PlayerSystem.Log.Info("Slash : " + onAttack.DamageData.Slash);

      if (!(onAttack.Target is NwCreature oTarget))
        return;

      await NwModule.Instance.WaitForObjectContext();

      pipeline.Execute(new Context(
        onAttack: onAttack,
        oTarget: oTarget
      ));
    }
    private static void HandleItemRuined(NwCreature oPC, NwItem oItem)
    {
      CreaturePlugin.RunUnequip(oPC, oItem);
      oItem.GetLocalVariable<int>("_DURABILITY").Value = -1;
      foreach (ItemProperty ip in oItem.ItemProperties.Where(ip => ip.Tag.StartsWith("ENCHANTEMENT")))
      {
        Task waitLoopEnd = NwTask.Run(async () =>
        {
          ItemProperty deactivatedIP = ip;
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          oItem.RemoveItemProperty(deactivatedIP);
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          deactivatedIP.Tag += "_INACTIVE";
          oItem.AddItemProperty(deactivatedIP, EffectDuration.Permanent);
        });
      }

      oPC.ControllingPlayer.SendServerMessage($"Il ne reste plus que des ruines de votre {oItem.Name.ColorString(ColorConstants.White)}. Des réparations s'imposent !", ColorConstants.Red);
    }

    private static void IsAttackDodged(Context ctx, Action next)
    {
      int skillBonusDodge = 0;

      if (PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player) && player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedDodge))
        skillBonusDodge += 2 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedDodge, player.learntCustomFeats[CustomFeats.ImprovedDodge]);

      if (ctx.oTarget.KnowsFeat(Feat.Dodge))
        skillBonusDodge += 2;

      int dodgeRoll = NwRandom.Roll(Utils.random, 100);

      if (dodgeRoll <= ctx.oTarget.GetAbilityModifier(Ability.Dexterity) + skillBonusDodge - ctx.oTarget.ArmorCheckPenalty - ctx.oTarget.ShieldCheckPenalty)
      {
        ctx.onAttack.AttackResult = AttackResult.Miss;
        if (ctx.onAttack.Attacker.IsPlayerControlled)
          ctx.onAttack.Attacker.ControllingPlayer.SendServerMessage($"{ctx.oTarget.Name} a esquivé votre attaque.");

        if (ctx.oTarget.IsPlayerControlled)
          ctx.oTarget.ControllingPlayer.SendServerMessage($"Attaque de {ctx.onAttack.Attacker.Name} esquivée.");
      }
      else
        next();
    }
    private static void ProcessAutomaticHit(Context ctx, Action next)
    {
      if (ctx.onAttack.AttackResult == AttackResult.Miss)
        ctx.onAttack.AttackResult = AttackResult.AutomaticHit;
    }
    private static void ProcessBaseDamageTypeAndAttackWeapon(Context ctx, Action next)
    {
      if (ctx.isUnarmedAttack)
      {
        ctx.baseDamageType = 2; // Bludgeoning

        if (ctx.onAttack.Attacker.GetItemInSlot(InventorySlot.Arms) != null)
          ctx.attackWeapon = ctx.onAttack.Attacker.GetItemInSlot(InventorySlot.Arms);
      }

      switch (ctx.onAttack.WeaponAttackType)
      {
        case WeaponAttackType.MainHand:

          if (ctx.onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand) != null)
          {
            ctx.attackWeapon = ctx.onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);
            ctx.baseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          }

          break;

        case WeaponAttackType.Offhand:

          if (ctx.onAttack.Attacker.GetItemInSlot(InventorySlot.LeftHand) != null)
          {
            ctx.attackWeapon = ctx.onAttack.Attacker.GetItemInSlot(InventorySlot.LeftHand);
            ctx.baseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          }

          break;

        case WeaponAttackType.CreatureBite:
          ctx.attackWeapon = ctx.onAttack.Attacker.GetItemInSlot(InventorySlot.CreatureBiteWeapon);
          ctx.baseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          break;

        case WeaponAttackType.CreatureLeft:
          ctx.attackWeapon = ctx.onAttack.Attacker.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
          ctx.baseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          break;

        case WeaponAttackType.CreatureRight:
          ctx.attackWeapon = ctx.onAttack.Attacker.GetItemInSlot(InventorySlot.CreatureRightWeapon);
          ctx.baseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          break;
      }

      next();
    }
    private static void ProcessTargetDamageAbsorption(Context ctx, Action next)
    {
      if (ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin) == null)
        next();

      ItemProperty absorption = null;
      ItemProperty secondaryAbsorption = null;
      int bonusAbsorbedDamage = 0;

      switch (ctx.baseDamageType)
      {
        case 1: //Piercing

          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 1 || i.SubType == 4) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption == null || (ctx.onAttack.DamageData.Base < 1 && ctx.onAttack.DamageData.Pierce < 1))
            break;

          switch (absorption.CostTableValue)
          {
            case 8:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Pierce / 4;
              ctx.onAttack.DamageData.Pierce -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 25);
              break;
            case 9:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Pierce / 2;
              ctx.onAttack.DamageData.Pierce -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 50);
              break;
            case 10:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Pierce * 3 / 4;
              ctx.onAttack.DamageData.Pierce -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 75);
              break;
            case 11:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Pierce;
              ctx.onAttack.DamageData.Pierce = 0;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 100);
              break;
          }

          break;

        case 2: //Bludgeoning

          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 0 || i.SubType == 4) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption == null || (ctx.onAttack.DamageData.Base < 1 && ctx.onAttack.DamageData.Bludgeoning < 1))
            break;

          switch (absorption.CostTableValue)
          {
            case 8:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Bludgeoning / 4;
              ctx.onAttack.DamageData.Bludgeoning -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 25);
              break;
            case 9:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Bludgeoning / 2;
              ctx.onAttack.DamageData.Bludgeoning -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 50);
              break;
            case 10:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Bludgeoning * 3 / 4;
              ctx.onAttack.DamageData.Bludgeoning -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 75);
              break;
            case 11:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Bludgeoning;
              ctx.onAttack.DamageData.Bludgeoning = 0;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 100);
              break;
          }

          break;

        case 3: //Slashing

          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 2 || i.SubType == 4) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption == null || (ctx.onAttack.DamageData.Base < 1 && ctx.onAttack.DamageData.Slash < 1))
            break;

          switch (absorption.CostTableValue)
          {
            case 8:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Slash / 4;
              ctx.onAttack.DamageData.Slash -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 25);
              break;
            case 9:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Slash / 2;
              ctx.onAttack.DamageData.Slash -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 50);
              break;
            case 10:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Slash * 3 / 4;
              ctx.onAttack.DamageData.Slash -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 75);
              break;
            case 11:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Slash;
              ctx.onAttack.DamageData.Slash = 0;
              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 100);
              break;
          }

          break;

        case 4: // Slashing and Piercing

          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 4) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption == null)
          {
            absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 1) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

            if (absorption == null)
              absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 2) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();
            else
            {
              secondaryAbsorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 2) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

              if (secondaryAbsorption != null)
              {
                if (absorption.CostTableValue > secondaryAbsorption.CostTableValue)
                  absorption = secondaryAbsorption;
              }
            }
          }

          if (absorption == null || (ctx.onAttack.DamageData.Base < 1 && ctx.onAttack.DamageData.Slash < 1 && ctx.onAttack.DamageData.Pierce < 1))
            break;

          switch (absorption.CostTableValue)
          {
            case 8:

              if (absorption.SubType == 4 || absorption.SubType == 2)
              {
                bonusAbsorbedDamage = ctx.onAttack.DamageData.Slash / 4;
                ctx.onAttack.DamageData.Slash -= (short)bonusAbsorbedDamage;
              }

              if (bonusAbsorbedDamage < 0)
                bonusAbsorbedDamage = 1;

              if (absorption.SubType == 4 || absorption.SubType == 1)
              {
                bonusAbsorbedDamage += ctx.onAttack.DamageData.Pierce / 4;
                ctx.onAttack.DamageData.Pierce -= (short)(ctx.onAttack.DamageData.Pierce / 4);
              }

              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 25);
              break;
            case 9:

              if (absorption.SubType == 4 || absorption.SubType == 2)
              {
                bonusAbsorbedDamage = ctx.onAttack.DamageData.Slash / 2;
                ctx.onAttack.DamageData.Slash -= (short)bonusAbsorbedDamage;
              }

              if (bonusAbsorbedDamage < 0)
                bonusAbsorbedDamage = 1;

              if (absorption.SubType == 4 || absorption.SubType == 1)
              {
                bonusAbsorbedDamage += ctx.onAttack.DamageData.Pierce / 2;
                ctx.onAttack.DamageData.Pierce -= (short)(ctx.onAttack.DamageData.Pierce / 2);
              }

              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 50);
              break;
            case 10:

              if (absorption.SubType == 4 || absorption.SubType == 2)
              {
                bonusAbsorbedDamage = ctx.onAttack.DamageData.Slash * 3 / 4;
                ctx.onAttack.DamageData.Slash -= (short)bonusAbsorbedDamage;
              }

              if (bonusAbsorbedDamage < 0)
                bonusAbsorbedDamage = 1;

              if (absorption.SubType == 4 || absorption.SubType == 1)
              {
                bonusAbsorbedDamage += ctx.onAttack.DamageData.Pierce * 3 / 4;
                ctx.onAttack.DamageData.Pierce -= (short)(ctx.onAttack.DamageData.Pierce * 3 / 4);
              }

              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 75);
              break;
            case 11:

              if (absorption.SubType == 4 || absorption.SubType == 2)
              {
                bonusAbsorbedDamage = ctx.onAttack.DamageData.Slash;
                ctx.onAttack.DamageData.Slash = 0;
              }

              if (bonusAbsorbedDamage < 0)
                bonusAbsorbedDamage = 1;

              if (absorption.SubType == 4 || absorption.SubType == 1)
              {
                bonusAbsorbedDamage += ctx.onAttack.DamageData.Pierce;
                ctx.onAttack.DamageData.Pierce = 0;
              }

              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 100);
              break;
          }

          break;

        case 5: // Bludgeoning and Piercing

          absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 4) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (absorption == null)
          {
            absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 1) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

            if (absorption == null)
              absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 0) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();
            else
            {
              secondaryAbsorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 0) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

              if (secondaryAbsorption != null)
              {
                if (absorption.CostTableValue > secondaryAbsorption.CostTableValue)
                  absorption = secondaryAbsorption;
              }
            }
          }

          if (absorption == null || (ctx.onAttack.DamageData.Base < 1 && ctx.onAttack.DamageData.Bludgeoning < 1 && ctx.onAttack.DamageData.Pierce < 1))
            break;

          switch (absorption.CostTableValue)
          {
            case 8:

              if (absorption.SubType == 4 || absorption.SubType == 0)
              {
                bonusAbsorbedDamage = ctx.onAttack.DamageData.Bludgeoning / 4;
                ctx.onAttack.DamageData.Bludgeoning -= (short)bonusAbsorbedDamage;
              }

              if (bonusAbsorbedDamage < 0)
                bonusAbsorbedDamage = 1;

              if (absorption.SubType == 4 || absorption.SubType == 1)
              {
                bonusAbsorbedDamage += ctx.onAttack.DamageData.Pierce / 4;
                ctx.onAttack.DamageData.Pierce -= (short)(ctx.onAttack.DamageData.Pierce / 4);
              }

              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 25);
              break;
            case 9:

              if (absorption.SubType == 4 || absorption.SubType == 0)
              {
                bonusAbsorbedDamage = ctx.onAttack.DamageData.Bludgeoning / 2;
                ctx.onAttack.DamageData.Bludgeoning -= (short)bonusAbsorbedDamage;
              }

              if (bonusAbsorbedDamage < 0)
                bonusAbsorbedDamage = 1;

              if (absorption.SubType == 4 || absorption.SubType == 1)
              {
                bonusAbsorbedDamage += ctx.onAttack.DamageData.Pierce / 2;
                ctx.onAttack.DamageData.Pierce -= (short)(ctx.onAttack.DamageData.Pierce / 2);
              }

              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 50);
              break;
            case 10:

              if (absorption.SubType == 4 || absorption.SubType == 0)
              {
                bonusAbsorbedDamage = ctx.onAttack.DamageData.Bludgeoning * 3 / 4;
                ctx.onAttack.DamageData.Bludgeoning -= (short)bonusAbsorbedDamage;
              }

              if (bonusAbsorbedDamage < 0)
                bonusAbsorbedDamage = 1;

              if (absorption.SubType == 4 || absorption.SubType == 1)
              {
                bonusAbsorbedDamage += ctx.onAttack.DamageData.Pierce * 3 / 4;
                ctx.onAttack.DamageData.Pierce -= (short)(ctx.onAttack.DamageData.Pierce * 3 / 4);
              }

              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 75);
              break;
            case 11:

              if (absorption.SubType == 4 || absorption.SubType == 0)
              {
                bonusAbsorbedDamage = ctx.onAttack.DamageData.Bludgeoning;
                ctx.onAttack.DamageData.Bludgeoning = 0;
              }

              if (bonusAbsorbedDamage < 0)
                bonusAbsorbedDamage = 1;

              if (absorption.SubType == 4 || absorption.SubType == 1)
              {
                bonusAbsorbedDamage += ctx.onAttack.DamageData.Pierce;
                ctx.onAttack.DamageData.Pierce = 0;
              }

              HandleDamageAbsorbed(ctx, ctx.onAttack.DamageData.Base, bonusAbsorbedDamage, 100);
              break;
          }

          break;
      }

      if (ctx.onAttack.DamageData.Acid > 0)
      {
        absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 6 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

        if (absorption != null)
        {
          switch (absorption.CostTableValue)
          {
            case 8:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Acid / 4;
              ctx.onAttack.DamageData.Acid -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
              break;
            case 9:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Acid / 2;
              ctx.onAttack.DamageData.Acid -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
              break;
            case 10:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Acid * 3 / 4;
              ctx.onAttack.DamageData.Acid -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
              break;
            case 11:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Acid;
              ctx.onAttack.DamageData.Acid = 0;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
              break;
          }
        }
      }

      if (ctx.onAttack.DamageData.Electrical > 0)
      {
        absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 9 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

        if (absorption != null)
        {
          switch (absorption.CostTableValue)
          {
            case 8:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Electrical / 4;
              ctx.onAttack.DamageData.Electrical -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
              break;
            case 9:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Electrical / 2;
              ctx.onAttack.DamageData.Electrical -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
              break;
            case 10:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Electrical * 3 / 4;
              ctx.onAttack.DamageData.Electrical -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
              break;
            case 11:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Electrical;
              ctx.onAttack.DamageData.Electrical = 0;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
              break;
          }
        }
      }

      if (ctx.onAttack.DamageData.Cold > 0)
      {
        absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 7 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

        if (absorption != null)
        {
          switch (absorption.CostTableValue)
          {
            case 8:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Cold / 4;
              ctx.onAttack.DamageData.Cold -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
              break;
            case 9:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Cold / 2;
              ctx.onAttack.DamageData.Cold -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
              break;
            case 10:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Cold * 3 / 4;
              ctx.onAttack.DamageData.Cold -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
              break;
            case 11:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Cold;
              ctx.onAttack.DamageData.Cold = 0;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
              break;
          }
        }
      }

      if (ctx.onAttack.DamageData.Fire > 0)
      {
        absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 10 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

        if (absorption != null)
        {
          switch (absorption.CostTableValue)
          {
            case 8:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Fire / 4;
              ctx.onAttack.DamageData.Fire -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
              break;
            case 9:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Fire / 2;
              ctx.onAttack.DamageData.Fire -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
              break;
            case 10:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Fire * 3 / 4;
              ctx.onAttack.DamageData.Fire -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
              break;
            case 11:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Fire;
              ctx.onAttack.DamageData.Fire = 0;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
              break;
          }
        }
      }

      if (ctx.onAttack.DamageData.Magical > 0)
      {
        absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 5 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

        if (absorption != null)
        {
          switch (absorption.CostTableValue)
          {
            case 8:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Magical / 4;
              ctx.onAttack.DamageData.Magical -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
              break;
            case 9:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Magical / 2;
              ctx.onAttack.DamageData.Magical -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
              break;
            case 10:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Magical * 3 / 4;
              ctx.onAttack.DamageData.Magical -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
              break;
            case 11:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Magical;
              ctx.onAttack.DamageData.Magical = 0;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
              break;
          }
        }
      }

      if (ctx.onAttack.DamageData.Sonic > 0)
      {
        absorption = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && (i.SubType == 13 || i.SubType == 14) && i.CostTableValue > 7).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

        if (absorption != null)
        {
          switch (absorption.CostTableValue)
          {
            case 8:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Sonic / 4;
              ctx.onAttack.DamageData.Sonic -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 25);
              break;
            case 9:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Sonic / 2;
              ctx.onAttack.DamageData.Sonic -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 50);
              break;
            case 10:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Sonic * 3 / 4;
              ctx.onAttack.DamageData.Sonic -= (short)bonusAbsorbedDamage;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 75);
              break;
            case 11:
              bonusAbsorbedDamage = ctx.onAttack.DamageData.Sonic;
              ctx.onAttack.DamageData.Sonic = 0;
              HandleDamageAbsorbed(ctx, 0, bonusAbsorbedDamage, 100);
              break;
          }
        }
      }

      next();
    }
    private static void HandleDamageAbsorbed(Context ctx, int baseDamage, int bonusDamage, int multiplier)
    {
      int absorbedDamage = 0;
      if (baseDamage < 1)
        baseDamage = 0;

      if (bonusDamage < 1)
        bonusDamage = 0;

      absorbedDamage += baseDamage * multiplier / 100;
      ctx.onAttack.DamageData.Base -= (short)absorbedDamage;

      absorbedDamage += bonusDamage;

      ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.Heal(absorbedDamage));
      ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHeal));

      foreach (NwCreature oPC in ctx.oTarget.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled && p.DistanceSquared(ctx.oTarget) < 35))
        oPC.ControllingPlayer.DisplayFloatingTextStringOnCreature(ctx.oTarget, absorbedDamage.ToString().ColorString(new Color(32, 255, 32)));
    }
    private static void ProcessBaseArmorPenetration(Context ctx, Action next)
    {
      if (ctx.onAttack.WeaponAttackType == WeaponAttackType.Offhand)
        ctx.baseArmorPenetration = CreaturePlugin.GetAttackBonus(ctx.onAttack.Attacker, -1, 1);
      else
        ctx.baseArmorPenetration = CreaturePlugin.GetAttackBonus(ctx.onAttack.Attacker);

      if (ctx.attackWeapon.BaseItemType == BaseItemType.Gloves) // la fonction CreaturePlugin.GetAttackBonus ne prend pas en compte le + AB des gants, donc je le rajoute
      {
        int glovesAttackBonus = 0;

        foreach (ItemProperty ip in ctx.attackWeapon.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AttackBonus || i.PropertyType == ItemPropertyType.EnhancementBonus))
          glovesAttackBonus += ip.CostTableValue;

        ctx.baseArmorPenetration += glovesAttackBonus;
      }

      next();
    }
    private static void ProcessBonusArmorPenetration(Context ctx, Action next)
    {
      if (ctx.attackWeapon == null)
        next();

      foreach (ItemProperty ip in ctx.attackWeapon.ItemProperties.Where(i => (i.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i.SubType == (int)ctx.oTarget.RacialType)
         || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.GoodEvilAlignment)
         || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.LawChaosAlignment)
         || (i.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oTarget))
         || (i.PropertyType == ItemPropertyType.EnhancementBonusVsRacialGroup && i.SubType == (int)ctx.oTarget.RacialType)
         || (i.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.GoodEvilAlignment)
         || (i.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.LawChaosAlignment)
         || (i.PropertyType == ItemPropertyType.EnhancementBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oTarget))))
        ctx.bonusArmorPenetration += ip.CostTableValue;

      next();
    }
    private static void ProcessAttackPosition(Context ctx, Action next)
    {
      if ((ctx.isUnarmedAttack || !ctx.isRangedAttack)
        && ctx.onAttack.Attacker.Size != ctx.oTarget.Size
        && !((ctx.onAttack.Attacker.Size == CreatureSize.Small && ctx.oTarget.Size == CreatureSize.Medium) || (ctx.onAttack.Attacker.Size == CreatureSize.Medium && ctx.oTarget.Size == CreatureSize.Small)))
      {
        if (ctx.onAttack.Attacker.Size > ctx.oTarget.Size)
          ctx.attackPosition = Config.AttackPosition.High;
        else
          ctx.attackPosition = Config.AttackPosition.Low;
      }

      next();
    }
    private static void ProcessArmorSlotHit(Context ctx, Action next)
    {
      InventorySlot hitSlot;
      int randLocation = NwRandom.Roll(Utils.random, 100);

      switch (ctx.attackPosition)
      {
        case Config.AttackPosition.NormalOrRanged:

          if (randLocation < 13)
            hitSlot = InventorySlot.Boots;
          else if (randLocation < 38)
            hitSlot = InventorySlot.Cloak;
          else if (randLocation < 51)
            hitSlot = InventorySlot.Arms;
          else if (randLocation < 88)
            hitSlot = InventorySlot.Chest;
          else
            hitSlot = InventorySlot.Head;

          break;

        case Config.AttackPosition.Low:

          if (randLocation < 28)
            hitSlot = InventorySlot.Boots;
          else if (randLocation < 69)
            hitSlot = InventorySlot.Cloak;
          else if (randLocation < 82)
            hitSlot = InventorySlot.Arms;
          else
            hitSlot = InventorySlot.Chest;

          break;

        case Config.AttackPosition.High:

          if (randLocation < 19)
            hitSlot = InventorySlot.Cloak;
          else if (randLocation < 33)
            hitSlot = InventorySlot.Arms;
          else if (randLocation < 74)
            hitSlot = InventorySlot.Chest;
          else
            hitSlot = InventorySlot.Head;

          break;

        default:
          return;
      }

      ctx.targetArmor = ctx.oTarget.GetItemInSlot(hitSlot);

      if (ctx.targetArmor == null && !ctx.oTarget.IsLoginPlayerCharacter) // Dans le cas où la créature n'a pas d'armure et n'est pas un joueur, alors on simplifie et on prend directement sa CA de base pour la CA générique et les propriétés de sa peau pour la CA spécifique
      {
        ctx.targetArmor = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin);
        ctx.targetAC[DamageType.BaseWeapon] = ctx.oTarget.AC;
      }

      next();
    }
    private static void ProcessTargetSpecificAC(Context ctx, Action next)
    {
      if (ctx.targetArmor == null)
        next();

      foreach (ItemProperty ip in ctx.targetArmor.ItemProperties.Where(i
       => i.PropertyType == ItemPropertyType.AcBonus
       || i.PropertyType == ItemPropertyType.AcBonusVsDamageType
       || (i.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i.SubType == (int)ctx.onAttack.Attacker.RacialType)
       || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.onAttack.Attacker.GoodEvilAlignment)
       || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.onAttack.Attacker.LawChaosAlignment)
       || (i.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.onAttack.Attacker))))
      {
        switch (ip.PropertyType)
        {
          case ItemPropertyType.AcBonusVsDamageType:

            switch (ip.SubType)
            {
              case 0:

                if (ctx.targetAC.ContainsKey(DamageType.Bludgeoning))
                  ctx.targetAC[DamageType.Bludgeoning] += ip.CostTableValue;
                else
                  ctx.targetAC.Add(DamageType.Bludgeoning, ip.CostTableValue);

                break;

              case 1:

                if (ctx.targetAC.ContainsKey(DamageType.Piercing))
                  ctx.targetAC[DamageType.Piercing] += ip.CostTableValue;
                else
                  ctx.targetAC.Add(DamageType.Piercing, ip.CostTableValue);

                break;

              case 2:

                if (ctx.targetAC.ContainsKey(DamageType.Slashing))
                  ctx.targetAC[DamageType.Slashing] += ip.CostTableValue;
                else
                  ctx.targetAC.Add(DamageType.Slashing, ip.CostTableValue);

                break;

              case 4: // All physical damage

                if (ctx.targetAC.ContainsKey((DamageType)4))
                  ctx.targetAC[(DamageType)4] += ip.CostTableValue;
                else
                  ctx.targetAC.Add((DamageType)4, ip.CostTableValue);

                break;

              case 5:

                if (ctx.targetAC.ContainsKey(DamageType.Magical))
                  ctx.targetAC[DamageType.Magical] += ip.CostTableValue;
                else
                  ctx.targetAC.Add(DamageType.Magical, ip.CostTableValue);

                break;

              case 6:

                if (ctx.targetAC.ContainsKey(DamageType.Acid))
                  ctx.targetAC[DamageType.Acid] += ip.CostTableValue;
                else
                  ctx.targetAC.Add(DamageType.Acid, ip.CostTableValue);

                break;

              case 7:

                if (ctx.targetAC.ContainsKey(DamageType.Cold))
                  ctx.targetAC[DamageType.Cold] += ip.CostTableValue;
                else
                  ctx.targetAC.Add(DamageType.Cold, ip.CostTableValue);

                break;

              case 9:

                if (ctx.targetAC.ContainsKey(DamageType.Electrical))
                  ctx.targetAC[DamageType.Electrical] += ip.CostTableValue;
                else
                  ctx.targetAC.Add(DamageType.Electrical, ip.CostTableValue);

                break;

              case 10:

                if (ctx.targetAC.ContainsKey(DamageType.Fire))
                  ctx.targetAC[DamageType.Fire] += ip.CostTableValue;
                else
                  ctx.targetAC.Add(DamageType.Fire, ip.CostTableValue);

                break;

              case 13:

                if (ctx.targetAC.ContainsKey(DamageType.Sonic))
                  ctx.targetAC[DamageType.Sonic] += ip.CostTableValue;
                else
                  ctx.targetAC.Add(DamageType.Sonic, ip.CostTableValue);

                break;

              case 14: // All elemental damage

                if (ctx.targetAC.ContainsKey((DamageType)14))
                  ctx.targetAC[(DamageType)14] += ip.CostTableValue;
                else
                  ctx.targetAC.Add((DamageType)14, ip.CostTableValue);

                break;
            }

            break;

          default:

            if (ctx.targetAC.ContainsKey(DamageType.BaseWeapon))
              ctx.targetAC[DamageType.BaseWeapon] += ip.CostTableValue;
            else
              ctx.targetAC.Add(DamageType.BaseWeapon, ip.CostTableValue);

            break;
        }
      }

      next();
    }
    private static void ProcessTargetShieldAC(Context ctx, Action next)
    {
      NwItem targetShield = ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand);

      if (targetShield == null || (targetShield != null && ItemUtils.GetItemCategory(targetShield.BaseItemType) != ItemUtils.ItemCategory.Shield))
        next();

      foreach (ItemProperty ip in targetShield.ItemProperties.Where(ip => ip.PropertyType == ItemPropertyType.AcBonus))
        ctx.targetAC[DamageType.BaseWeapon] += ip.CostTableValue;

      foreach (ItemProperty ip in targetShield.ItemProperties.Where(ip => ip.PropertyType == ItemPropertyType.AcBonusVsDamageType && ip.SubType == 1)) // 1 = piercing
        if (ctx.targetAC.ContainsKey(DamageType.Piercing))
          ctx.targetAC[DamageType.Piercing] += ip.CostTableValue;
        else
          ctx.targetAC.Add(DamageType.Piercing, ip.CostTableValue);

      next();
    }
    private static void ProcessArmorPenetrationCalculations(Context ctx, Action next)
    {
      ctx.targetAC[DamageType.BaseWeapon] = ctx.targetAC[DamageType.BaseWeapon] * (100 - ctx.baseArmorPenetration - ctx.bonusArmorPenetration) / 100;

      if (ctx.targetAC[DamageType.BaseWeapon] < 0)
        ctx.targetAC[DamageType.BaseWeapon] = 0;

      next();
    }
    private static void ProcessDamageCalculations(Context ctx, Action next)
    {
      if (ctx.onAttack.DamageData.Base > 0)
      {
        int bonusAC = 0;

        switch (ctx.baseDamageType)
        {
          case 1: // Piercing
            ctx.onAttack.DamageData.Base = (short)(ctx.onAttack.DamageData.Base * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Piercing) - 60) / 40));
            break;
          case 2: // Bludgeoning
            ctx.onAttack.DamageData.Base = (short)(ctx.onAttack.DamageData.Base * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Bludgeoning) - 60) / 40));
            break;
          case 3: // Slashing
            ctx.onAttack.DamageData.Base = (short)(ctx.onAttack.DamageData.Base * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Slashing) - 60) / 40));
            break;
          case 4: // Slashing and Piercing

            if (ctx.targetAC.GetValueOrDefault(DamageType.Slashing) > ctx.targetAC.GetValueOrDefault(DamageType.Piercing))
              bonusAC = ctx.targetAC.GetValueOrDefault(DamageType.Piercing);
            else
              bonusAC = ctx.targetAC.GetValueOrDefault(DamageType.Slashing);

            ctx.onAttack.DamageData.Base = (short)(ctx.onAttack.DamageData.Base * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + bonusAC - 60) / 40));

            break;
          case 5: //Piercing and bludgeoning

            if (ctx.targetAC.GetValueOrDefault(DamageType.Bludgeoning) > ctx.targetAC.GetValueOrDefault(DamageType.Piercing))
              bonusAC = ctx.targetAC.GetValueOrDefault(DamageType.Piercing);
            else
              bonusAC = ctx.targetAC.GetValueOrDefault(DamageType.Bludgeoning);

            ctx.onAttack.DamageData.Base = (short)(ctx.onAttack.DamageData.Base * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + bonusAC - 60) / 40));

            break;
        }
      }

      if (ctx.onAttack.DamageData.Bludgeoning > 0)
        ctx.onAttack.DamageData.Bludgeoning = (short)(ctx.onAttack.DamageData.Bludgeoning * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Bludgeoning) - 60) / 40));

      if (ctx.onAttack.DamageData.Pierce > 0)
        ctx.onAttack.DamageData.Pierce = (short)(ctx.onAttack.DamageData.Pierce * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Piercing) - 60) / 40));

      if (ctx.onAttack.DamageData.Slash > 0)
        ctx.onAttack.DamageData.Slash = (short)(ctx.onAttack.DamageData.Slash * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)4) + ctx.targetAC.GetValueOrDefault(DamageType.Slashing) - 60) / 40));

      if (ctx.onAttack.DamageData.Electrical > 0)
        ctx.onAttack.DamageData.Electrical = (short)(ctx.onAttack.DamageData.Electrical * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Electrical) - 60) / 40));

      if (ctx.onAttack.DamageData.Acid > 0)
        ctx.onAttack.DamageData.Acid = (short)(ctx.onAttack.DamageData.Acid * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Acid) - 60) / 40));

      if (ctx.onAttack.DamageData.Cold > 0)
        ctx.onAttack.DamageData.Cold = (short)(ctx.onAttack.DamageData.Cold * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Cold) - 60) / 40));

      if (ctx.onAttack.DamageData.Fire > 0)
        ctx.onAttack.DamageData.Fire = (short)(ctx.onAttack.DamageData.Fire * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Fire) - 60) / 40));

      if (ctx.onAttack.DamageData.Magical > 0)
        ctx.onAttack.DamageData.Magical = (short)(ctx.onAttack.DamageData.Magical * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Magical) - 60) / 40));

      if (ctx.onAttack.DamageData.Sonic > 0)
        ctx.onAttack.DamageData.Sonic = (short)(ctx.onAttack.DamageData.Sonic * Math.Pow(0.5, (ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)14) + ctx.targetAC.GetValueOrDefault(DamageType.Sonic) - 60) / 40));

      ctx.oTarget.GetLocalVariable<int>("_DAMAGE_HANDLED").Value = 1;

      next();
    }
    private static void ProcessAttackerItemDurability(Context ctx, Action next)
    {
      if (!PlayerSystem.Players.TryGetValue(ctx.onAttack.Attacker, out PlayerSystem.Player attacker))
        next();

      // L'attaquant est un joueur. On diminue la durabilité de son arme

      if (ctx.attackWeapon == null)
        next();

      int durabilityChance = 30;

      int dexBonus = ctx.onAttack.Attacker.GetAbilityModifier(Ability.Dexterity) - (CreaturePlugin.GetArmorCheckPenalty(ctx.onAttack.Attacker) + CreaturePlugin.GetShieldCheckPenalty(ctx.onAttack.Attacker));
      if (dexBonus < 0)
        dexBonus = 0;

      int safetyLevel = 0;
      if (attacker.learntCustomFeats.ContainsKey(CustomFeats.CombattantPrecautionneux))
        safetyLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.CombattantPrecautionneux, attacker.learntCustomFeats[CustomFeats.CombattantPrecautionneux]);

      durabilityChance -= dexBonus + safetyLevel;

      if (NwRandom.Roll(Utils.random, 100, 1) < 2 && NwRandom.Roll(Utils.random, 100, 1) < durabilityChance)
      {
        ctx.attackWeapon.GetLocalVariable<int>("_DURABILITY").Value -= 1;
        if (ctx.attackWeapon.GetLocalVariable<int>("_DURABILITY").Value <= 0)
        {
          if (ctx.attackWeapon.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasNothing)
          {
            ctx.attackWeapon.Destroy();
            attacker.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {ctx.attackWeapon.Name.ColorString(ColorConstants.White)}. Ces débris ne sont même pas réparables !", ColorConstants.Red);
          }
          else
            HandleItemRuined(ctx.onAttack.Attacker, ctx.attackWeapon);
        }
      }

      next();
    }
    private static void ProcessTargetItemDurability(Context ctx, Action next)
    {
      if (!PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player))
        next();

      // La cible de l'attaque est un joueur, on fait diminuer la durabilité

      int dexBonus = ctx.oTarget.GetAbilityModifier(Ability.Dexterity) - (CreaturePlugin.GetArmorCheckPenalty(ctx.oTarget) + CreaturePlugin.GetShieldCheckPenalty(ctx.oTarget));
      if (dexBonus < 0)
        dexBonus = 0;

      int safetyLevel = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.CombattantPrecautionneux))
        safetyLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.CombattantPrecautionneux, player.learntCustomFeats[CustomFeats.CombattantPrecautionneux]);

      int durabilityRate = 30 - dexBonus - safetyLevel;
      if (durabilityRate < 1)
        durabilityRate = 1;

      if (NwRandom.Roll(Utils.random, 100, 1) > 1 && NwRandom.Roll(Utils.random, 100, 1) > durabilityRate)
        return;

      int random = NwRandom.Roll(Utils.random, 11, 1) - 1;
      int loop = random + 1;
      NwItem item = ctx.oTarget.GetItemInSlot((InventorySlot)random);

      while (item == null && loop != random)
      {
        if (loop > 10)
          loop = 0;

        item = ctx.oTarget.GetItemInSlot((InventorySlot)loop);
        loop++;
      }

      if (item == null || item.Tag == "amulettorillink")
        return;

      item.GetLocalVariable<int>("_DURABILITY").Value -= 1;
      if (item.GetLocalVariable<int>("_DURABILITY").Value <= 0)
      {
        if (item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasNothing)
        {
          item.Destroy();
          player.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {item.Name.ColorString(ColorConstants.White)}. Ces débris ne sont même pas réparables !", ColorConstants.Red);
        }
        else
          HandleItemRuined(ctx.oTarget, item);
      }

      next();
    }
  }
}
