using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CourrouxDeLaNatureEffectTag = "_COURROUX_DE_LA_NATURE_EFFECT";
    public static readonly Native.API.CExoString CourrouxDeLaNatureEffectExoTag = CourrouxDeLaNatureEffectTag.ToExoString();
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

      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.AnciensCourrouxDeLaNature), Ability.Charisma);
      Ability saveAbility = target.GetAbilityModifier(Ability.Strength) > target.GetAbilityModifier(Ability.Dexterity) ? Ability.Strength : Ability.Dexterity;
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, saveAbility, Spells2da.spellTable[CustomSpell.AnciensCourrouxDeLaNature], SpellConfig.SpellEffectType.Invalid);

      if (advantage < -900)
        return ScriptHandleResult.Handled;

      int totalSave = SpellUtils.GetSavingThrowRoll(target, saveAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, saveAbility);

      if(!saveFailed)
        target.RemoveEffect(eventData.Effect);

      return ScriptHandleResult.Handled;
    }
  }
}
