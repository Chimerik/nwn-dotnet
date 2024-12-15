using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ExpertiseDesarmement(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Whip))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackExpertiseDesarmement;
        caster.OnCreatureAttack += CreatureUtils.OnAttackExpertiseDesarmement;

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Désarmement", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDesarmement, 0);
    }
  }
}
