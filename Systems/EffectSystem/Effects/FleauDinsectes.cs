using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FleauDinsectesAOEEffectTag = "_FLEAU_DINSECTES_AOE_EFFECT";
    public const string FleauDinsectesEffectTag = "_FLEAU_DINSECTES_EFFECT";
    private static ScriptCallbackHandle onEnterFleauDinsectesCallback;
    private static ScriptCallbackHandle onExitFleauDinsectesCallback;
    private static ScriptCallbackHandle onHeartbeatFleauDinsectesCallback;
    public static Effect FleauDinsectesAoE(NwCreature caster)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerCreepingDoom, onEnterFleauDinsectesCallback, onHeartbeatFleauDinsectesCallback, onExitFleauDinsectesCallback);
      eff.Tag = FleauDinsectesAOEEffectTag;
      eff.Spell = NwSpell.FromSpellId(CustomSpell.FleauDinsectes);
      eff.Creator = caster;
      return eff;
    }
    public static Effect FleauDinsectes
    {
      get
      {
        Effect eff = Effect.MovementSpeedDecrease(50);
        eff.Tag = FleauDinsectesEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterFleauDinsectes(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering)
        return ScriptHandleResult.Handled;

      if(eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      if(!entering.ActiveEffects.Any(e => e.Tag == FleauDinsectesEffectTag))
        NWScript.AssignCommand(caster, () => entering.ApplyEffect(EffectDuration.Permanent, FleauDinsectes));

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.FleauDinsectes];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, (Ability)caster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{eventData.Effect.Spell.Id}").Value);

      FleauDinsectesDamage(caster, entering, spellEntry, spellDC);
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatFleauDinsectes(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData))
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.FleauDinsectes];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, (Ability)caster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{eventData.Effect.Spell.Id}").Value);

      foreach(NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>()) 
      {
        FleauDinsectesDamage(caster, entering, spellEntry, spellDC);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitFleauDinsectes(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, FleauDinsectesEffectTag);
      return ScriptHandleResult.Handled;
    }
    private static void FleauDinsectesDamage(NwCreature caster, NwCreature entering, SpellEntry spellEntry, int spellDC)
    {
      SpellConfig.SavingThrowFeedback feedback = new();

      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(entering, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, caster);
      int totalSave = SpellUtils.GetSavingThrowRoll(entering, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, entering, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

      if (saveFailed)
        SpellUtils.DealSpellDamage(entering, caster.CasterLevel, spellEntry, spellEntry.numDice, caster, 5);
    }
  }
}
