using Anvil.API;
using NWN.Native.API;
using DamageType = Anvil.API.DamageType;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ManifestationAmeEffectTag = "_MANIFESTATION_AME_EFFECT";
    public static readonly CExoString ManifestationAmeEffectExoTag = ManifestationAmeEffectTag.ToExoString();
    public static Effect GetMonkManifestationAmeEffect(int wisdomModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.DamageIncrease(6, DamageType.Divine), Effect.DamageIncrease(wisdomModifier, DamageType.Divine));
      eff.Tag = ManifestationAmeEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
