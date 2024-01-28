using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string UnarmoredDefenceEffectTag = "_UNARMORED_DEFENSE_EFFECT";
    public static Effect GetUnarmoredDefenseEffect(int constitutionModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.ACIncrease(constitutionModifier, ACBonus.ArmourEnchantment), Effect.Icon(EffectIcon.ACIncrease));
      eff.Tag = UnarmoredDefenceEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
