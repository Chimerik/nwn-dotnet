using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void CogneurLourd(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.CogneurLourdEffectTag))
      {
        foreach (var effect in caster.ActiveEffects)
          if (effect.Tag == EffectSystem.CogneurLourdEffectTag)
            caster.RemoveEffect(effect);
      }
      else
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.cogneurLourdEffect);
    }
  }
}
