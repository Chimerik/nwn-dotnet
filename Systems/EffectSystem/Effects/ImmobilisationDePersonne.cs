using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalImmobilisationDePersonneCallback;
    public const string ImmobilisationDePersonneEffectTag = "_IMMOBILISATION_DE_PERSONNE_EFFECT";
    public static Effect GetImmobilisationDePersonneEffect(Ability spellCastingAbility, NwSpell spell)
    {
      Effect eff = Effect.LinkEffects(Effect.Paralyze(), Effect.VisualEffect(VfxType.DurParalyzeHold),
        Effect.RunAction(onIntervalHandle: onIntervalImmobilisationDePersonneCallback, interval: TimeSpan.FromSeconds(6), data:((int)spellCastingAbility).ToString()));
      eff.Tag = ImmobilisationDePersonneEffectTag;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult OnIntervalImmobilisationDePersonne(CallInfo callInfo)
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

      SpellEntry spellEntry = Spells2da.spellTable[(int)Spell.HoldPerson];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, Spell.HoldPerson, (Ability)int.Parse(eff.StringParams[0]));

      if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry, SpellConfig.SpellEffectType.Paralysis) != SavingThrowResult.Failure)
        target.RemoveEffect(eff);

      return ScriptHandleResult.Handled;
    }
  }
}
