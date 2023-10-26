using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect heavyArmorSlow;
    public static readonly string heavyArmorSlowEffectTag = "_EFFECT_HEAVY_ARMOR_SLOW";

    public static void InitHeavyArmorSlowEffect()
    {
      heavyArmorSlow = Effect.LinkEffects(Effect.MovementSpeedDecrease(30), Effect.Icon(NwGameTables.EffectIconTable.GetRow(38)));
      heavyArmorSlow.Tag = heavyArmorSlowEffectTag;
      heavyArmorSlow.SubType = EffectSubType.Unyielding;
    }
  }
}
