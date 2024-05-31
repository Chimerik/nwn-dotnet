using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HurlementGalvanisantEffectTag = "_HURLEMENT_GALVANISANT_EFFECT";
    public static Effect hurlementGalvanisant
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(15));
        eff.Tag = HurlementGalvanisantEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
