using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceDraconiqueEffectTag = "_RESISTANCE_DRACONIQUE_EFFECT";
    public static Effect GetResistanceDraconiqueEffect(int charismaModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.ACIncrease(charismaModifier, ACBonus.ArmourEnchantment), Effect.Icon(EffectIcon.ACIncrease));
      eff.Tag = ResistanceDraconiqueEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
