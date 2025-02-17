using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalConfusionCallback;
    public const string ConfusionEffectTag = "_CONFUSION_EFFECT";
    public static Effect GetConfusionEffect(Ability spellCastingAbility, NwSpell spell)
    {
      Effect eff = Effect.LinkEffects(Effect.Confused(), 
        Effect.RunAction(onIntervalHandle: onIntervalConfusionCallback, interval: TimeSpan.FromSeconds(6), data:((int)spellCastingAbility).ToString()));
      eff.Tag = ConfusionEffectTag;
      eff.Spell = spell;
      return eff;
    }
    private static ScriptHandleResult OnIntervalConfusion(CallInfo callInfo)
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

      SpellEntry spellEntry = Spells2da.spellTable[(int)Spell.Confusion];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, Spell.Confusion, (Ability)int.Parse(eff.StringParams[0]));

      if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility,  spellDC, spellEntry) != SavingThrowResult.Failure)
        target.RemoveEffect(eff);

      return ScriptHandleResult.Handled;
    }
  }
}
