using System.Numerics;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkFrappeDombre(NwCreature caster, OnUseFeat onFeat)
    {
      if(onFeat.TargetObject is not NwCreature target || caster == target)
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez choisir une cible valide", ColorConstants.Red);
        return;
      }

      if (caster.GetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkFrappeDombre)) < 3)
      {
        caster.LoginPlayer?.SendServerMessage("Cette attaque nécessite 3 charges de Ki", ColorConstants.Red);
        return;
      }

      if (target.IsCreatureSeen(caster))
      {
        caster.LoginPlayer?.SendServerMessage("La cible peut vous voir", ColorConstants.Red);
        return;
      }

      if(caster.DistanceSquared(target) > 320)
      {
        caster.LoginPlayer?.SendServerMessage("La cible est hors de portée", ColorConstants.Red);
        return;
      }

      NwItem torch = target.GetItemInSlot(InventorySlot.LeftHand);

      if (torch is not null && torch.BaseItem.ItemType == BaseItemType.Torch)
      {
        caster.LoginPlayer?.SendServerMessage("La cible est trop en lumière pour que vous puissiez vous téléporter dans son ombre", ColorConstants.Red);
        return;
      }

      foreach (var eff in target.ActiveEffects)
        if (eff.EffectType == EffectType.VisualEffect)
          if (eff.IntParams[0] == 148 || (eff.IntParams[0] > 152 && eff.IntParams[0] < 181))
          {
            caster.LoginPlayer?.SendServerMessage("La cible est trop en lumière pour que vous puissiez vous téléporter dans son ombre", ColorConstants.Red);
            return;
          }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe des ombres sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);

      Vector3 safePosition = CreaturePlugin.ComputeSafeLocation(ModuleSystem.placeholderTemplate, target.Position, 2);

      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      caster.Position = safePosition;
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));

      target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 8, 3), CustomDamageType.Psychic));

      FeatUtils.DecrementKi(caster);
      FeatUtils.DecrementKi(caster);
      FeatUtils.DecrementKi(caster);
    }
  }
}
