using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect heavyArmorSlow
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedDecrease(30), Effect.Icon(NwGameTables.EffectIconTable.GetRow(38)));
        eff.Tag = heavyArmorSlowEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public static readonly string heavyArmorSlowEffectTag = "_EFFECT_HEAVY_ARMOR_SLOW";
  }
}
