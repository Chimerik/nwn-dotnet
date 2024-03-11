using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MonkUnarmoredDefenceEffectTag = "_MONK_UNARMORED_DEFENSE_EFFECT";
    public static Effect GetMonkUnarmoredDefenseEffect(int wisdomModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.ACIncrease(wisdomModifier, ACBonus.ArmourEnchantment), Effect.Icon(EffectIcon.ACIncrease));
      eff.Tag = MonkUnarmoredDefenceEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
