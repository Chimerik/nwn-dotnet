using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string NageffectTag = "_NAGE_EFFECT";

    public static Effect Nage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = NageffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

