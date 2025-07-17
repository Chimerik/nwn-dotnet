using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CourrouxDeLaNatureEffectTag = "_COURROUX_DE_LA_NATURE_EFFECT";
    private static ScriptCallbackHandle onIntervalCourrouxDeLaNatureCallback;
    
    public static Effect CourrouxDeLaNature
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurEntangle), Effect.CutsceneImmobilize(),
          Effect.RunAction(onIntervalHandle: onIntervalCourrouxDeLaNatureCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = CourrouxDeLaNatureEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalCourrouxDeLaNature(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if(eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is not NwCreature caster)
      {
        target.RemoveEffect(eventData.Effect);
        return ScriptHandleResult.Handled;
      }

      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.AnciensCourrouxDeLaNature), Ability.Charisma);
      Ability saveAbility = target.GetAbilityModifier(Ability.Strength) > target.GetAbilityModifier(Ability.Dexterity) ? Ability.Strength : Ability.Dexterity;
      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.AnciensCourrouxDeLaNature];

      if(CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC) != SavingThrowResult.Failure)
        target.RemoveEffect(eventData.Effect);

      return ScriptHandleResult.Handled;
    }
  }
}
