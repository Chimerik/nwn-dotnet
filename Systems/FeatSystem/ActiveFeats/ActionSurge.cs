using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ActionSurge(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ActionSurgeEffectTag))
        return;

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.actionSurgeEffect, NwTimeSpan.FromRounds(10));
      FeatUtils.DecrementFeatUses(caster, CustomSkill.FighterSurge);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Fougue Martiale".ColorString(ColorConstants.White)}", ColorConstants.Orange, true);
    }
  }
}
