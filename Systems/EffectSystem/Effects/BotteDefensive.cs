using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BotteDefensiveEffectTag = "_BOTTE_DEFENSIVE_EFFECT";
    public static Effect GetBotteDefensiveEffect(int bonus)
    {
      Effect eff = Effect.LinkEffects(Effect.ACIncrease(bonus), Effect.Icon(EffectIcon.ACIncrease));
      eff.Tag = BotteDefensiveEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
