using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AntidetectionEffectTag = "_ANTIDETECTION_EFFECT";
    public static Effect Antidetection
    {
      get
      {
        Effect eff = Effect.Icon((EffectIcon)179);
        eff.Tag = AntidetectionEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
