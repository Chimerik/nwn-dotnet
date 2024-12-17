using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MonkUnarmoredDefenceEffectTag = "_MONK_UNARMORED_DEFENSE_EFFECT";
    public static Effect GetMonkUnarmoredDefenseEffect(int wisdomModifier)
    {
      Effect acInc = Effect.ACIncrease(wisdomModifier, ACBonus.ArmourEnchantment);
      acInc.ShowIcon = false;

      Effect eff = Effect.LinkEffects(acInc, Effect.Icon(CustomEffectIcon.BarbareDefenseSansArmure));
      eff.Tag = MonkUnarmoredDefenceEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
