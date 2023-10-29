using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DwarfSlowEffectTag = "_DWARF_SLOW_EFFECT";
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
