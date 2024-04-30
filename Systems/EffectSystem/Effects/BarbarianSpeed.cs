using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BarbarianSpeedEffectTag = "_BARBARIAN_SPEED_EFFECT";
    public static Effect BarbarianSpeed
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(15), Effect.Icon(EffectIcon.MovementSpeedIncrease));
        eff.Tag = BarbarianSpeedEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
