using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string UnarmoredDefenceEffectTag = "_UNARMORED_DEFENSE_EFFECT";
    public static Effect GetUnarmoredDefenseEffect(int constitutionModifier)
    {
      Effect acInc = Effect.ACIncrease(constitutionModifier, ACBonus.ArmourEnchantment);
      acInc.ShowIcon = false;

      Effect eff = Effect.LinkEffects(acInc, Effect.Icon(CustomEffectIcon.BarbareDefenseSansArmure));
      eff.Tag = UnarmoredDefenceEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
