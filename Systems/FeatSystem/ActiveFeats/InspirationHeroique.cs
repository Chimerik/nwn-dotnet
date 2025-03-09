using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void InspirationHeroique(NwCreature caster)
    {
      if (!caster.ActiveEffects.Any(e => e.Tag == EffectSystem.InspirationBardiqueEffectTag))
      {
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHoly));

        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.InspirationHeroique);
        caster.DecrementRemainingFeatUses((Feat)CustomSkill.InspirationHeroique);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Vous disposez déjà des effets d'inspiration héroïque", ColorConstants.Orange);
    }
  }
}
