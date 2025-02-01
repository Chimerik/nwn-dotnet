using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PreparationEffectTag = "_PREPARATION_EFFECT";
    public static Effect Preparation
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Preparation), Effect.MovementSpeedDecrease(75));
        eff.Tag = PreparationEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

