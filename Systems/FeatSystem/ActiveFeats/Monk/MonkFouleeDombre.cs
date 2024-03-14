﻿using System.Numerics;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkFouleeDombre(NwCreature caster, OnUseFeat onFeat)
    {
      if(onFeat.TargetObject is not NwCreature target || caster == target)
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez choisir une cible valide", ColorConstants.Red);
        return;
      }

      if(!NwModule.Instance.IsNight || !caster.Location.Area.IsInterior)
      {
        caster.LoginPlayer?.SendServerMessage("Il faut attendre la nuit, ou bien être en intérieur pour utiliser cette capacité", ColorConstants.Red);
        return;
      }

      if(caster.DistanceSquared(target) > 320)
      {
        caster.LoginPlayer?.SendServerMessage("La cible est hors de portée", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Foulée des ombres sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);

      Vector3 safePosition = CreaturePlugin.ComputeSafeLocation(ModuleSystem.placeholderTemplate, target.Position, 2);

      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      caster.Position = safePosition;
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));

      NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || !weapon.BaseItem.IsRangedWeapon)
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.FouleeDombre, NwTimeSpan.FromRounds(1));
    }
  }
}
