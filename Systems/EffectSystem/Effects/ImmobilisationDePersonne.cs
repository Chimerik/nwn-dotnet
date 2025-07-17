using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalImmobilisationDePersonneCallback;
    public const string ImmobilisationDePersonneEffectTag = "_IMMOBILISATION_DE_PERSONNE_EFFECT";
    public static Effect GetImmobilisationDePersonneEffect(Ability spellCastingAbility, NwSpell spell)
    {
      Effect eff = Effect.LinkEffects(Effect.Paralyze(), Effect.VisualEffect(VfxType.DurParalyzeHold),
        Effect.RunAction(onIntervalHandle: onIntervalImmobilisationDePersonneCallback, interval: NwTimeSpan.FromRounds(1)));
      eff.Tag = ImmobilisationDePersonneEffectTag;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = (int)spellCastingAbility;
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
      int spellDC = SpellUtils.GetCasterSpellDC(caster, Spell.HoldPerson, (Ability)eff.CasterLevel);

      if (CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC, spellEntry, SpellConfig.SpellEffectType.Paralysis) != SavingThrowResult.Failure)
        target.RemoveEffect(eff);

      return ScriptHandleResult.Handled;
    }
  }
}
