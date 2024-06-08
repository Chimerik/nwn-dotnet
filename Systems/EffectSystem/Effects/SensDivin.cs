using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SensDivinEffectTag = "_SENS_DIVIN_EFFECT";
    public static readonly Native.API.CExoString sensDivinEffectExoTag = SensDivinEffectTag.ToExoString();
    public static Effect SensDivin
    {
      get
      {
        Effect eff = Effect.Icon((EffectIcon)173);
        eff.Tag = SensDivinEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
