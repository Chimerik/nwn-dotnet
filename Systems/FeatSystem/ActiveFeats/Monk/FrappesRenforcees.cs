using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappesRenforcees(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.FrappesRenforceesEffectTag))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.FrappesRenforceesEffectTag);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));
      }
      else
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FrappesRenforcees(caster));
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));
      }
    }
  }
}
