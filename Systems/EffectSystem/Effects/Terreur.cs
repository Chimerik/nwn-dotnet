﻿using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalTerreurCallback;
    public const string TerreurEffectTag = "_TERREUR_EFFECT";
    public static Effect GetTerreurEffect(Ability ability)
    {
      Effect eff = Effect.LinkEffects(Effect.Frightened(), 
        Effect.RunAction(onIntervalHandle: onIntervalTerreurCallback, interval: NwTimeSpan.FromRounds(1), data:((int)ability).ToString()));
      eff.Tag = TerreurEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult OnIntervalTerreur(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if(eventData.EffectTarget is not NwCreature target || eventData.Effect.Creator is not NwCreature caster || target.HasLineOfSight(caster))
        return ScriptHandleResult.Handled;

      SpellConfig.SavingThrowFeedback feedback = new();
      SpellEntry spellEntry = Spells2da.spellTable[(int)Spell.Fear];
      Ability castingAbility = (Ability)int.Parse(eventData.Effect.StringParams[0]);
      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellType(Spell.Fear), castingAbility);
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Fear, caster);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

      if (saveFailed)
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetTerreurEffect(castingAbility), NwTimeSpan.FromRounds(spellEntry.duration)));

      return ScriptHandleResult.Handled;
    }
  }
}
