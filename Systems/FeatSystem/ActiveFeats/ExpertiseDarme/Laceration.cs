using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Laceration(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.TwoBladedSword, BaseItemType.Bastardsword, BaseItemType.Katana, BaseItemType.Longsword, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackLaceration;
        caster.OnCreatureAttack += CreatureUtils.OnAttackLaceration;

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadEvil));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Lacération", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseLaceration, 0);
    }
  }
}
