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

      Effect AC = Effect.ACIncrease(3 + wisModifier, ACBonus.ArmourEnchantment);
      AC.ShowIcon = false;

      var eff = Effect.LinkEffects(AC, Effect.Icon(CustomEffectIcon.FormeDeLune), Effect.TemporaryHitpoints(druidLevel * 3));
        
      eff.Tag = FormeDeLuneEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
