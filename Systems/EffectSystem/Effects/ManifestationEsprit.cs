using Anvil.API;
using NWN.Native.API;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ManifestationEspritEffectTag = "_MANIFESTATION_ESPRIT_EFFECT";
    public static readonly CExoString ManifestationEspritEffectExoTag = ManifestationEspritEffectTag.ToExoString();
    public static Effect GetMonkManifestationEspritEffect(int wisdomModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.DamageIncrease(6, CustomDamageType.Psychic), Effect.DamageIncrease(wisdomModifier, CustomDamageType.Psychic));
      eff.Tag = ManifestationEspritEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
