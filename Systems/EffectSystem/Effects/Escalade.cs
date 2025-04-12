using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EscaladeEffectTag = "_ESCALADE_EFFECT";

    public static Effect Escalade(bool unyielding = false)
    {
      Effect eff = Effect.RunAction();
      eff.Tag = EscaladeEffectTag;
      eff.SubType = unyielding ? EffectSubType.Unyielding : EffectSubType.Supernatural;
      return eff;
    }
  }
}

