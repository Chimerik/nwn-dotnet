
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Renforcement(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.ShortSpear, BaseItemType.Halberd, BaseItemType.TwoBladedSword))
      {
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadFire));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Renforcement, NwTimeSpan.FromRounds(1));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 60, CustomSkill.ExpertiseRenforcement), NwTimeSpan.FromRounds(10));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Renforcement", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseRenforcement, 0);
    }
  }
}
