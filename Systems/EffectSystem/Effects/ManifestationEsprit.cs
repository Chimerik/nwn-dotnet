using System;
using Anvil.API;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ManifestationEspritEffectTag = "_MANIFESTATION_ESPRIT_EFFECT";
    public static Effect GetMonkManifestationEspritEffect(int wisdomModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.DamageIncrease(6, CustomDamageType.Psychic), Effect.DamageIncrease(wisdomModifier, CustomDamageType.Psychic),
        Effect.RunAction(onIntervalHandle: onIntervalManifestationCallback, interval: TimeSpan.FromSeconds(2)));
      eff.Tag = ManifestationEspritEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
