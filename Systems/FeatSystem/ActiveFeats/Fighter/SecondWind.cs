using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SecondWind(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      int fighterLevel = FighterUtils.GetFighterLevel(caster);
      caster.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Heal(NwRandom.Roll(Utils.random, 10, 1) + fighterLevel), Effect.VisualEffect(VfxType.ImpHealingM)));
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.FighterSecondWind);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Second Souffle".ColorString(ColorConstants.White)}", ColorConstants.Orange, true);
    }
  }
}
