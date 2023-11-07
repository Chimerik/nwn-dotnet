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
        eff.ShowIcon = false;
        eff.Tag = DwarfSlowEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
