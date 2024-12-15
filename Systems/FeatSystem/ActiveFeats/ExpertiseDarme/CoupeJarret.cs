using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void CoupeJarret(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortbow, BaseItemType.Longbow, BaseItemType.ThrowingAxe, BaseItemType.Dart))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackCoupeJarret;
        caster.OnCreatureAttack += CreatureUtils.OnAttackCoupeJarret;

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadNature));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Coupe Jarret", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCoupeJarret, 0);
    }
  }
}
