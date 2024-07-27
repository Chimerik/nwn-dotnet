using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BourrasqueEffectTag = "_BOURRASQUE_EFFECT";
    public const string BourrasqueSlowEffectTag = "_BOURRASQUE_SLOW_EFFECT";
    private static ScriptCallbackHandle onEnterBourrasqueCallback;
    private static ScriptCallbackHandle onExitBourrasqueCallback;
    private static ScriptCallbackHandle onHeartbeatBourrasqueCallback;
    public static Effect Bourrasque(NwCreature caster)
    {
      Effect eff = Effect.AreaOfEffect((PersistentVfxType)257, onEnterBourrasqueCallback, onHeartbeatBourrasqueCallback, onExitBourrasqueCallback);
      eff.Tag = BourrasqueEffectTag;
      eff.Spell = NwSpell.FromSpellType(Spell.GustOfWind);
      eff.Creator = caster;
      return eff;
    }
    public static Effect BourrasqueSlow
    {
      get
      {
        Effect eff = Effect.MovementSpeedDecrease(50);
        eff.Tag = BourrasqueSlowEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterBourrasque(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData))
      {
        if (eventData.Entering is NwCreature entering)
        {
          if (eventData.Effect.Creator is not NwCreature caster)
          {
            eventData.Effect.Destroy();
            return ScriptHandleResult.Handled;
          }

          if (!entering.ActiveEffects.Any(e => e.Tag == BourrasqueSlowEffectTag))
            NWScript.AssignCommand(caster, () => entering.ApplyEffect(EffectDuration.Permanent, BourrasqueSlow));

          if (entering != caster)
          {
            SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.FleauDinsectes];
            int spellDC = SpellUtils.GetCasterSpellDC(caster, (Ability)caster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{eventData.Effect.Spell.Id}").Value);
            BourrasqueKnockdown(caster, entering, spellEntry, spellDC);
          }
        }
        else if (eventData.Entering is NwAreaOfEffect aoe)
          aoe.Destroy();
      }
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatBourrasque(CallInfo callInfo)
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
        if(entering != caster)
          BourrasqueKnockdown(caster, entering, spellEntry, spellDC);
      }

      foreach (NwAreaOfEffect aoe in eventData.Effect.GetObjectsInEffectArea<NwAreaOfEffect>())
        aoe.Destroy();

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitBourrasque(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, BourrasqueSlowEffectTag);
      return ScriptHandleResult.Handled;
    }
    private static void BourrasqueKnockdown(NwCreature caster, NwCreature entering, SpellEntry spellEntry, int spellDC)
    {
      SpellConfig.SavingThrowFeedback feedback = new();

      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(entering, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Knockdown, caster);
      int totalSave = SpellUtils.GetSavingThrowRoll(entering, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, entering, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

      if (saveFailed)
      {
        ApplyKnockdown(entering, CreatureSize.Large, 2);
        entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseWind));
      }
    }
  }
}
