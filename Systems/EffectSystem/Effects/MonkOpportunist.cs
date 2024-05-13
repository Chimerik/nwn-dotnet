using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MonkOpportunistEffectTag = "_MONK_OPPORTUNIST_EFFECT";
    public static readonly Native.API.CExoString monkOpportunistEffectExoTag = MonkOpportunistEffectTag.ToExoString();
    public static Effect MonkOpportunist
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = MonkOpportunistEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
