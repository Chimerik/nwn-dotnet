using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MageDeGuerre(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MageDeGuerreEffectTag))
      {
        foreach (var effect in caster.ActiveEffects)
          if (effect.Tag == EffectSystem.MageDeGuerreEffectTag)
            caster.RemoveEffect(effect);
      }
      else
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.MageDeGuerreEffect);
    }
  }
}
