using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect mobile
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(15);
        eff.Tag = mobileEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public const string mobileEffectTag = "_MOBILE_EFFECT";
    public static Effect mobileDebuff
    { 
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = mobileDebuffEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public const string mobileDebuffEffectTag = "_MOBILE_DEBUFF_EFFECT";
  }
}
