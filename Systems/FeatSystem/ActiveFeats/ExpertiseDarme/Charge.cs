using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Charge(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Bastardsword, BaseItemType.Katana, BaseItemType.Longsword, BaseItemType.Halberd, BaseItemType.ShortSpear))
      {
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadSonic));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 60, CustomSkill.ExpertiseCharge), NwTimeSpan.FromRounds(10));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Charge(caster), NwTimeSpan.FromRounds(1));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Charge", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCharge, 0);
    }
  }
}
