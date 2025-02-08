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
        Effect speed = Effect.MovementSpeedDecrease(75);
        speed.ShowIcon = false;

        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Preparation), speed);
        eff.Tag = PreparationEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

