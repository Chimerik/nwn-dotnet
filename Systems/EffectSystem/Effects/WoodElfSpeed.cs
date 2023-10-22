using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect woodElfSpeed;

    public static void InitWoodElfSpeedEffect()
    {
      woodElfSpeed = Effect.MovementSpeedIncrease(10);
      woodElfSpeed.SubType = EffectSubType.Unyielding;
      woodElfSpeed.Tag = "_WOODELF_SPEED_EFFECT";
    }
  }
}
