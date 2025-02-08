using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Entaille(NwCreature caster)
    {
      var secondWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

      if (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, secondWeapon) && Utils.In(secondWeapon.BaseItem.ItemType, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Handaxe, BaseItemType.Scimitar, BaseItemType.LightHammer, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
      {
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Entaille, NwTimeSpan.FromRounds(2));
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 60, CustomSkill.ExpertiseEntaille), NwTimeSpan.FromRounds(10));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Entaille", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseEntaille, 0);
    }
  }
}
