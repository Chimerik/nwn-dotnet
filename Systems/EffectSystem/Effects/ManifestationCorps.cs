using System;
using Anvil.API;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ManifestationCorpsEffectTag = "_MANIFESTATION_CORPS_EFFECT";
    public static Effect GetMonkManifestationCorpsEffect(int wisdomModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.DamageIncrease(6, CustomDamageType.Necrotic), Effect.DamageIncrease(wisdomModifier, CustomDamageType.Necrotic),
        Effect.RunAction(onIntervalHandle: onIntervalManifestationCallback, interval: TimeSpan.FromSeconds(2)));
      eff.Tag = ManifestationCorpsEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
