using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SurchargeArcanique(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.EvocateurSurchargeEffectTag))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.EvocateurSurchargeEffectTag);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDispel));
        caster.LoginPlayer?.SendServerMessage("Surcharge Arcanique désactivée", ColorConstants.Orange);
      }
      else
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.EvocateurSurcharge);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
        caster.LoginPlayer?.SendServerMessage("Surcharge Arcanique activée", ColorConstants.Orange);
      }
    }
  }
}
