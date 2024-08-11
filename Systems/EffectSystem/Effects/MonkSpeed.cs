using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MonkSpeedEffectTag = "_MONK_SPEED_EFFECT";
    public static Effect GetMonkSpeedEffect(int monkLevel)
    {
      int speed = monkLevel > 17 ? 50 : monkLevel > 13 ? 40  : monkLevel > 9 ? 30 : monkLevel > 5 ? 20 : monkLevel > 1 ? 10 : 0;
      Effect eff = /*speed > 0 ?*/ Effect.LinkEffects(Effect.MovementSpeedIncrease(speed), Effect.Icon(EffectIcon.MovementSpeedIncrease))
          /*: Effect.RunAction()*/;
      eff.Tag = MonkSpeedEffectTag;
      eff.SubType = EffectSubType.Unyielding;

      return eff;
    }
  }
}
