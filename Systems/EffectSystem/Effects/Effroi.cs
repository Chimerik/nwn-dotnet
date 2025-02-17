using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrightenedEffectTag = "_FRIGHTENED_EFFECT";
    private static ScriptCallbackHandle onRemoveEffroiCallback;
    private static ScriptCallbackHandle onIntervalEffroiCallback;
    public static void ApplyEffroi(NwCreature target, NwCreature caster, TimeSpan duration, bool repeatSave = false, NwSpell spell = null)
    {
      if (IsFrightImmune(target, caster))
        return;

      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingFear), Effect.Icon((EffectIcon)183));

      eff = repeatSave ? Effect.LinkEffects(eff, Effect.RunAction(onRemovedHandle: onRemoveEffroiCallback, onIntervalHandle: onIntervalEffroiCallback, interval: NwTimeSpan.FromRounds(1)))
        : Effect.LinkEffects(eff, Effect.RunAction(onRemovedHandle: onRemoveEffroiCallback));        

      eff.Tag = FrightenedEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.Spell = spell;

      target.GetObjectVariable<LocalVariableInt>("_PREVIOUS_MOVEMENT_RATE").Value = (int)target.MovementRate;
      target.MovementRate = MovementRate.Immobile;

      target.ApplyEffect(EffectDuration.Temporary, eff, duration);
    }
    public static bool IsFrightImmune(NwCreature target, NwCreature caster)
    {
      if (Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct) || target.ActiveEffects.Any(e => e.EffectType == EffectType.Immunity && e.IntParams[1] == 4) 
        || (target.KnowsFeat((Feat)CustomSkill.BersekerRageAveugle) && target.ActiveEffects.Any(e => e.Tag == BarbarianRageEffectTag)))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} est immunisé contre l'effroi");
        return true;
      }

      return false;
    }
    private static ScriptHandleResult OnRemoveEffroi(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
      {
        target.MovementRate = (MovementRate)target.GetObjectVariable<LocalVariableInt>("_PREVIOUS_MOVEMENT_RATE").Value;
        target.GetObjectVariable<LocalVariableInt>("_PREVIOUS_MOVEMENT_RATE").Delete();
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnIntervalEffroi(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target && eventData.Effect.Creator is NwCreature caster)
      {
        int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Charisma);

        if (CreatureUtils.GetSavingThrow(caster, target, Ability.Wisdom, spellDC, effectType: SpellConfig.SpellEffectType.Fear) != SavingThrowResult.Failure)
          target.RemoveEffect(eventData.Effect);
      }
      else if (eventData.EffectTarget is NwGameObject oTarget)
        oTarget.RemoveEffect(eventData.Effect);

      return ScriptHandleResult.Handled;
    }
  }
}
