using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Affaiblissement(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.HeavyFlail, BaseItemType.Rapier, BaseItemType.Whip, BaseItemType.Sling))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, secondWeapon) && ItemUtils.IsCreatureWeaponExpert(caster, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.Rapier, BaseItemType.Whip)))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackAffaiblissement;
        caster.OnCreatureAttack += CreatureUtils.OnAttackAffaiblissement;

        caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseAffaiblissement, 0);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadAcid));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Affaiblissement", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseAffaiblissement, 0);
    }
  }
}
