using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect dwarfSlow;

    public static void InitDwarfSlowEffect()
    {
      dwarfSlow = Effect.MovementSpeedDecrease(10);
      dwarfSlow.ShowIcon = false;
      dwarfSlow.SubType = EffectSubType.Unyielding;
    }
  }
}
