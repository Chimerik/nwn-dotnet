using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalLenteurCallback;
    public const string LenteurEffectTag = "_LENTEUR_EFFECT";
    public static Effect Lenteur(Ability spellCastingAbility)
    {
      Effect eff = Effect.LinkEffects(Effect.Slow(), noReactions, NoBonusAction,
        Effect.RunAction(onIntervalHandle: onIntervalLenteurCallback, interval: TimeSpan.FromSeconds(6), data:((int)spellCastingAbility).ToString()));
      eff.Tag = LenteurEffectTag;
      return eff;
    }
    private static ScriptHandleResult OnIntervalLenteur(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();
      Effect eff = eventData.Effect;
      
      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if (eff.Creator is not NwCreature caster)
      {
        target.RemoveEffect(eff);
        return ScriptHandleResult.Handled;
      }

      SpellEntry spellEntry = Spells2da.spellTable[(int)Spell.Slow];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, Spell.Slow, (Ability)int.Parse(eff.StringParams[0]));

      if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) != SavingThrowResult.Failure)
        target.RemoveEffect(eff);

      return ScriptHandleResult.Handled;
    }
  }
}
