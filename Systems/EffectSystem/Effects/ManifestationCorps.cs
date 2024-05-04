using Anvil.API;
using NWN.Native.API;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ManifestationCorpsEffectTag = "_MANIFESTATION_CORPS_EFFECT";
    public static readonly CExoString ManifestationCorpsEffectExoTag = ManifestationCorpsEffectTag.ToExoString();
    public static Effect GetMonkManifestationCorpsEffect(int wisdomModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.DamageIncrease(6, CustomDamageType.Necrotic), Effect.DamageIncrease(wisdomModifier, CustomDamageType.Necrotic));
      eff.Tag = ManifestationCorpsEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
