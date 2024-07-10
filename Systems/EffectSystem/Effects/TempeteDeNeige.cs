using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TempeteDeNeigeEffectTag = "_TEMPETE_DE_NEIGE_EFFECT";
    private static ScriptCallbackHandle onEnterTempeteDeNeigeCallback;
    private static ScriptCallbackHandle onHeartbeatTempeteDeNeigeCallback;
    public static Effect TempeteDeNeige
    {
      get
      {
        Effect eff = Effect.AreaOfEffect((PersistentVfxType)61, onEnterHandle: onEnterTempeteDeNeigeCallback, heartbeatHandle: onHeartbeatTempeteDeNeigeCallback);
        eff.Tag = TempeteDeNeigeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterTempeteDeNeige(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering)
        return ScriptHandleResult.Handled;

      if(eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.TempeteDeNeige];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.TempeteDeNeige), (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_SPELL_CASTING_ABILITY").Value);

      TempeteDeNeigeKnockDown(caster, entering, spellEntry, spellDC);
      TempeteDeNeigeConcentration(caster, entering, spellEntry, spellDC);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatTempeteDeNeige(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData))
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.TempeteDeNeige];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.TempeteDeNeige), (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_SPELL_CASTING_ABILITY").Value);

      foreach(NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>()) 
      { 
        TempeteDeNeigeKnockDown(caster, entering, spellEntry, spellDC);
        TempeteDeNeigeConcentration(caster, entering, spellEntry, spellDC);
      }

      return ScriptHandleResult.Handled;
    }
    private static void TempeteDeNeigeKnockDown(NwCreature caster, NwCreature entering, SpellEntry spellEntry, int spellDC)
    {
      SpellConfig.SavingThrowFeedback feedback = new();
      
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(entering, Ability.Dexterity, spellEntry, SpellConfig.SpellEffectType.Knockdown, caster);

      if (advantage < -900)
        return;

      int totalSave = SpellUtils.GetSavingThrowRoll(entering, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, entering, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

      if (saveFailed)
        entering.ApplyEffect(EffectDuration.Temporary, Effect.Knockdown(), NwTimeSpan.FromRounds(2));
    }
    private static void TempeteDeNeigeConcentration(NwCreature caster, NwCreature entering, SpellEntry spellEntry, int spellDC)
    {
      if (entering.ActiveEffects.Any(e => e.Tag == ConcentrationEffectTag))
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(entering, Ability.Constitution, spellEntry, SpellConfig.SpellEffectType.Invalid, caster);
        int totalSave = SpellUtils.GetSavingThrowRoll(entering, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(caster, entering, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

        if (saveFailed)
          SpellUtils.DispelConcentrationEffects(entering);
      }
    }
  }
}
