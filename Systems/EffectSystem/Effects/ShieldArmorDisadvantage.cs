using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect shieldArmorDisadvantage
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(EffectIcon.AttackDecrease));
        eff.Tag = ShieldArmorDisadvantageEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public const string ShieldArmorDisadvantageEffectTag = "_DISADVANTAGE_SHIELD_ARMOR_PROFICIENCY";
  }
}
