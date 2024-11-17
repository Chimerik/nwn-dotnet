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
        Effect eff = Effect.MovementSpeedIncrease(25);
        eff.Tag = AthleteAccompliEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

