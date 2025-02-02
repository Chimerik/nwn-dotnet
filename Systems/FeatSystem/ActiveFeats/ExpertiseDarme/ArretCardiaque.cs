using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ArretCardiaque(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightMace, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.HeavyFlail))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightMace, BaseItemType.Club, BaseItemType.Morningstar)))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackArretCardiaque;
        caster.OnCreatureAttack += CreatureUtils.OnAttackArretCardiaque;

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadCold));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Arrêt Cardiaque", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseArretCardiaque, 0);
    }
  }
}
