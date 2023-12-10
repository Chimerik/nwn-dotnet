using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TireurDelite(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.TireurDeliteEffectTag))
      {
        foreach (var effect in caster.ActiveEffects)
          if (effect.Tag == EffectSystem.TireurDeliteEffectTag)
            caster.RemoveEffect(effect);
      }
      else
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.TireurDeliteEffect);
    }
  }
}
