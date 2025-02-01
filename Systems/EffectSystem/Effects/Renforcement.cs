using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RenforcementEffectTag = "_RENFORCEMENT_EFFECT";
    public static Effect Renforcement
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Renforcement), Effect.MovementSpeedDecrease(75));
        eff.Tag = RenforcementEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

