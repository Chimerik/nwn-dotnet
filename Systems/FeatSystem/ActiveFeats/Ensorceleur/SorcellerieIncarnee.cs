using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SorcellerieIncarnee(NwCreature caster)
    {
      if(caster.GetFeatRemainingUses((Feat)CustomSkill.SorcellerieIncarnee) > 0)
      {
        caster.LoginPlayer?.SendServerMessage("Usage de Sorcellerie Incarnee disponible", ColorConstants.Orange);
        return;
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Sorcellerie Incarnée", StringUtils.gold, true, true);
      caster.IncrementRemainingFeatUses((Feat)CustomSkill.SorcellerieInnee);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRestorationLesser));

      EnsoUtils.DecrementSorcerySource(caster, 2);
    }
  }
}
