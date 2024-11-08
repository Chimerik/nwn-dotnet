using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VolEffectTag = "_VOL_EFFECT";
    public static readonly Native.API.CExoString VolEffectExoTag = VolEffectTag.ToExoString();
    public static Effect Vol
    {
      get
      {
        Effect eff = Effect.Icon((EffectIcon)222);
        eff.Tag = VolEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

