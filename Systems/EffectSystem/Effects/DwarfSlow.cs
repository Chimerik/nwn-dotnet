using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect dwarfSlow
    {
      get
      {
        Effect eff = Effect.MovementSpeedDecrease(10);
        dwarfSlow.ShowIcon = false;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
