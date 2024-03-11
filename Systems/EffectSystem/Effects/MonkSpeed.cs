using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MonkSpeedEffectTag = "_MONK_SPEED_EFFECT";
    public static Effect GetMonkSpeedEffect(int monkLevel)
    {
      int speed = monkLevel > 17 ? 60 : monkLevel > 13 ? 50 : monkLevel > 9 ? 40 : monkLevel > 5 ? 30 : monkLevel > 1 ? 20 : 0;
      Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(speed), Effect.Icon(EffectIcon.MovementSpeedIncrease));
      eff.Tag = MonkSpeedEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
