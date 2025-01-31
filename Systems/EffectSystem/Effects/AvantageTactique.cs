using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AvantageTactiqueEffectTag = "_AVANTAGE_TACTIQUE_EFFECT";
    public static Effect AvantageTactique
    {
      get
      {
        Effect speed = Effect.MovementSpeedIncrease(25);
        speed.ShowIcon = false;
        Effect eff = Effect.LinkEffects(speed, Effect.Icon(CustomEffectIcon.AvantageTactique));
        eff.Tag = AvantageTactiqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

