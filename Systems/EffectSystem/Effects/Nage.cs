using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string NageffectTag = "_NAGE_EFFECT";

    public static Effect Nage(bool unyielding = false)
    {
      Effect eff = Effect.RunAction();
      eff.Tag = NageffectTag;
      eff.SubType = unyielding ? EffectSubType.Unyielding : EffectSubType.Supernatural;
      return eff;
    }
  }
}

