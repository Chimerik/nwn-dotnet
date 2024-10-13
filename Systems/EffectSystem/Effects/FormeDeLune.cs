using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FormeDeLuneEffectTag = "_FORME_DE_LUNE_EFFECT";
    public static Effect FormeDeLune(NwCreature druide)
    {
      int wisModifier = druide.GetAbilityModifier(Ability.Wisdom) > 0 ? druide.GetAbilityModifier(Ability.Wisdom) : 0;
      int druidLevel = druide.GetClassInfo(ClassType.Druid) is null ? 0 : druide.GetClassInfo(ClassType.Druid).Level;

      Effect eff = Effect.LinkEffects(Effect.ACIncrease(3 + wisModifier, ACBonus.ArmourEnchantment), Effect.Icon((EffectIcon)186),
        Effect.TemporaryHitpoints(druidLevel * 3));
        
      eff.Tag = ProtectionNaturelleEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
