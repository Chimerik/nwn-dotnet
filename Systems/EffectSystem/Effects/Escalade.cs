using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EscaladeEffectTag = "_NAGE_EFFECT";
    public static readonly Native.API.CExoString EscaladeEffectExoTag = EscaladeEffectTag.ToExoString();
    public static Effect Escalade
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = EscaladeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

