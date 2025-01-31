using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AthleteAccompliEffectTag = "_ATHLETE_ACCOMPLI_EFFECT";
    public static Effect AthleteAccompli
    {
      get
      {
        Effect speed = Effect.MovementSpeedIncrease(25);
        speed.ShowIcon = false;
        Effect eff = Effect.LinkEffects(speed, Effect.Icon(CustomEffectIcon.AthleteAccompli));
        eff.Tag = AthleteAccompliEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

