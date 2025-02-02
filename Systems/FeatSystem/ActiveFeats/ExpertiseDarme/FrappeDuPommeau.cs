using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappeDuPommeau(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Longsword, BaseItemType.Bastardsword, BaseItemType.Greatsword))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackFrappeDuPommeau;
        caster.OnCreatureAttack += CreatureUtils.OnAttackFrappeDuPommeau;

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe du Pommeau", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFrappeDuPommeau, 0);
    }
  }
}
