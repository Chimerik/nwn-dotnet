﻿using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LenteurEffectTag = "_LENTEUR_EFFECT";
    private static ScriptCallbackHandle onIntervalLenteurCallback;
    private static ScriptCallbackHandle onRemoveLenteurCallback;
    public static void ApplyLenteur(NwCreature target, NwGameObject caster, NwSpell spell, Ability spellCastingAbility, TimeSpan duration)
    {
      target.OnSpellCast -= OnSpellCastLenteur;
      target.OnSpellCast += OnSpellCastLenteur;

      Effect eff = Effect.LinkEffects(Effect.Slow(), noReactions(target), NoBonusAction(target),
        Effect.RunAction(onRemovedHandle:onRemoveLenteurCallback, onIntervalHandle: onIntervalLenteurCallback, interval: TimeSpan.FromSeconds(6)));
      eff.CasterLevel = (int)spellCastingAbility;
      eff.Tag = LenteurEffectTag;
      eff.Spell = spell;
      eff.Creator = caster;

      target.ApplyEffect(EffectDuration.Temporary,eff, duration);
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
      int spellDC = SpellUtils.GetCasterSpellDC(caster, Spell.Slow, (Ability)eff.CasterLevel);

      if (CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC, spellEntry) != SavingThrowResult.Failure)
        target.RemoveEffect(eff);

      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult OnRemoveLenteur(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.OnSpellCast -= OnSpellCastLenteur;
      }

      return ScriptHandleResult.Handled;
    }

    public static void OnSpellCastLenteur(OnSpellCast onCast)
    {
      if (onCast.Caster is not NwCreature caster)
        return;

      if(Spells2da.spellTable[onCast.Spell.Id].requiresSomatic && Utils.Roll(4) < 2)
      {
        onCast.PreventSpellCast = true;
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSlow));
        caster.LoginPlayer?.SendServerMessage($"Lenteur : échec de {StringUtils.ToWhitecolor(onCast.Spell.Name.ToString())}", ColorConstants.Orange);
      }
    }
  }
}
