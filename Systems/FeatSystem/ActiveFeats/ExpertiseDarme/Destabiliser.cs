using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Destabiliser(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(caster, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Quarterstaff, BaseItemType.MagicStaff, BaseItemType.Whip))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackDestabiliser;
        caster.OnCreatureAttack += CreatureUtils.OnAttackDestabiliser;

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadSonic));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Déstabiliser", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDestabiliser, 0);
    }
  }
}
