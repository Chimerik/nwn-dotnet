using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect shieldArmorDisadvantage;
    public const string ShieldArmorDisadvantageEffectTag = "_DISADVANTAGE_SHIELD_ARMOR_PROFICIENCY";
    public static readonly Native.API.CExoString shieldArmorDisadvantageEffectExoTag = "_DISADVANTAGE_SHIELD_ARMOR_PROFICIENCY".ToExoString();

    public static void InitShieldArmorDisadvantageEffect()
    {
      shieldArmorDisadvantage = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(34)));
      shieldArmorDisadvantage.Tag = ShieldArmorDisadvantageEffectTag;
      shieldArmorDisadvantage.SubType = EffectSubType.Unyielding;
    }
  }
}
