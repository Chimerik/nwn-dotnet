using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect shieldArmorDisadvantage;

    public static void InitShieldArmorDisadvantageEffect()
    {
      shieldArmorDisadvantage = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(34)));
      shieldArmorDisadvantage.Tag = StringUtils.shieldArmorDisadvantageEffectTag;
      shieldArmorDisadvantage.SubType = EffectSubType.Unyielding;
    }
  }
}
