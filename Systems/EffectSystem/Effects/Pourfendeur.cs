using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PourfendeurSlowEffectTag = "_POURFENDEUR_SLOW_EFFECT";
    public static Effect PourfendeurSlowEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedDecrease(30), Effect.Icon(CustomEffectIcon.PourfendeurSlow));
        eff.Tag = PourfendeurSlowEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public const string PourfendeurDisadvantageEffectTag = "_POURFENDEUR_DISADVANTAGE_EFFECT";
    public static Effect PourfendeurDisadvantageEffect
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PourfendeurDisadvantage);
        eff.Tag = PourfendeurDisadvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
