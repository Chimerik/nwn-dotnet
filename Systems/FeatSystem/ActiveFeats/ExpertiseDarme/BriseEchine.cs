using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BriseEchine(NwCreature caster)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.DireMace, BaseItemType.Warhammer))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackBriseEchine;
        caster.OnCreatureAttack += CreatureUtils.OnAttackBriseEchine;

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadSonic));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Brise-Echine", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'une arme compatible avec cette expertise", ColorConstants.Orange);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseBriseEchine, 0);
    }
  }
}
