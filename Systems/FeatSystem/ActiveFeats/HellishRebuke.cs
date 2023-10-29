using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void HellishRebuke(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.HellishRebukeEffectTag))
      {
        foreach (var effect in caster.ActiveEffects)
          if (effect.Tag == EffectSystem.HellishRebukeEffectTag)
            caster.RemoveEffect(effect);
      }
      else
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.hellishRebukeEffect);
    }
  }
}
