using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect woodElfSpeed;
    public static readonly string woodElfEffectTag = "_WOODELF_SPEED_EFFECT";

    public static void InitWoodElfSpeedEffect()
    {
      woodElfSpeed = Effect.MovementSpeedIncrease(10);
      woodElfSpeed.ShowIcon = false;
      woodElfSpeed.SubType = EffectSubType.Unyielding;
      woodElfSpeed.Tag = woodElfEffectTag;
    }
  }
}
