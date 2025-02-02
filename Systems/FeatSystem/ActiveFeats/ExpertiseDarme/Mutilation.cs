using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Mutilation(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe, BaseItemType.Battleaxe, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Scythe))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe)))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackMutilation;
        caster.OnCreatureAttack += CreatureUtils.OnAttackMutilation;

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadEvil));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Mutilation", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMutilation, 0);
    }
  }
}
