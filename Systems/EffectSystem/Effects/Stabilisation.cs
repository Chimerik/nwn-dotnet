using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string StabilisationEffectTag = "_STABILISATION_EFFECT";
    public static readonly Native.API.CExoString StabilisationEffectExoTag = StabilisationEffectTag.ToExoString();
    public static Effect Stabilisation
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Stabilisation), Effect.MovementSpeedDecrease(75));
        eff.Tag = StabilisationEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

