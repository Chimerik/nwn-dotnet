﻿using Anvil.API;
using NativeUtils = NWN.Systems.NativeUtils;
using NWN.Systems;
using NWN.Core;
using System;
using System.Numerics;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void HandleTirPerforant(NwCreature caster)
    {
      if (ItemUtils.HasBowEquipped(caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType))
      {
        caster.LoginPlayer?.EnterTargetMode(SelectTirPerforantTarget, Config.selectLocationTargetMode);
        caster.LoginPlayer?.SendServerMessage("Tir Perforant - Veuillez choisir une cible", ColorConstants.Orange);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'un arc", ColorConstants.Red);
    }
    private static void SelectTirPerforantTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled)
        return;

      if (ItemUtils.HasBowEquipped(selection.Player.ControlledCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType))
      {
        if (selection.Player.ControlledCreature.GetObjectVariable<LocalVariableInt>(TirArcaniqueCooldownVariable).HasNothing)
        {
          Location targetLocation = Location.Create(selection.Player.ControlledCreature.Area, new Vector3(selection.TargetPosition.X, selection.TargetPosition.Y, selection.TargetPosition.Z + 1), selection.Player.ControlledCreature.Rotation + 180);
          targetLocation.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamChain, selection.Player.ControlledCreature, BodyNode.Hand), TimeSpan.FromSeconds(1));

          foreach (var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCylinder, 9, false))
            DealTirPerforantDamage(selection.Player.ControlledCreature, target);

          StringUtils.DelayLocalVariableDeletion<LocalVariableInt>(selection.Player.ControlledCreature, TirArcaniqueCooldownVariable, NwTimeSpan.FromRounds(1));
        }
        else
          selection.Player.SendServerMessage("Tir arcanique limité à un par round", ColorConstants.Red);
      }
      else
        selection.Player.SendServerMessage("Vous devez être équipé d'un arc", ColorConstants.Red);
    }
    private static void DealTirPerforantDamage(NwCreature caster, NwCreature target)
    {
      SpellConfig.SavingThrowFeedback feedback = new();
      int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Intelligence);
      int advantage = GetCreatureAbilityAdvantage(target, Ability.Dexterity);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Dexterity, tirDC, advantage, feedback);
      bool saveFailed = totalSave < tirDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Constitution);

      NwBaseItem weapon = caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem;
      int damage = NativeUtils.HandleWeaponDamageRerolls(caster, weapon, weapon.NumDamageDice, weapon.DieToRoll);
      damage /= saveFailed ? 2 : 1;

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(damage, DamageType.Piercing)));

      int forceDamage = caster.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter))?.Level < 18
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      forceDamage /= saveFailed ? 2 : 1;

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(forceDamage, DamageType.Magical)));

      FeatUtils.DecrementTirArcanique(caster);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Tir Perforant", StringUtils.gold, true);
    }
  }
}
