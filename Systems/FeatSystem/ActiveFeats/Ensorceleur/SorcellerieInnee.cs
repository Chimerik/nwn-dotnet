using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SorcellerieInnee(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.SorcellerieInnee);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Sorcellerie Innée", StringUtils.gold, true, true);
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.SorcellerieInnee, NwTimeSpan.FromRounds(10));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
    }
  }
}
