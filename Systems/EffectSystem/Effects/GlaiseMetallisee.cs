using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string GlaiseMetalliseeEffectTag = "_GLAISE_METALLISEE_EFFECT";
    public static Effect GlaiseMetallisee
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ACIncrease(2), Effect.MovementSpeedDecrease(75));
        eff.Tag = GlaiseMetalliseeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
