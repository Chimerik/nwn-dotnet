using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Fendre(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Battleaxe, BaseItemType.Doubleaxe, BaseItemType.Greatsword, BaseItemType.TwoBladedSword, BaseItemType.Greataxe, BaseItemType.DwarvenWaraxe, BaseItemType.Halberd, BaseItemType.Scythe))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Battleaxe, BaseItemType.DwarvenWaraxe)))
      {
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FendreAttackVariable).Value = 1;
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadSonic));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 60, CustomSkill.ExpertiseFendre), NwTimeSpan.FromRounds(10));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Fendre", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFendre, 0);
    }
  }
}
