using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void CogneurLourd(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.CogneurLourdEffectTag))
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.CogneurLourdEffectTag);
      else
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.cogneurLourdEffect);
    }
  }
}
