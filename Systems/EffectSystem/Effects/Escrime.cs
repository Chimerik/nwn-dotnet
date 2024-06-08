using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EscrimeEffectTag = "_ESCRIME_EFFECT";
    public static Effect Escrime
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(15), Effect.Icon(EffectIcon.MovementSpeedIncrease));
        eff.Tag = EscrimeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
