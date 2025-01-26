using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FouRireDeTashaEffectTag = "_FOU_RIRE_DE_TASHA_EFFECT";
    private static ScriptCallbackHandle onIntervalFouRireDeTashaCallback;
    private static ScriptCallbackHandle onRemoveFouRireDeTashaCallback;
    public static void ApplyFouRireDeTasha(NwCreature target, NwCreature caster, TimeSpan duration, Ability SaveAbility, Ability DCAbility)
    {
      //if (!IsSleepImmune(target, caster))
      //{
      int spellDC = SpellUtils.GetCasterSpellDC(caster, DCAbility);

      if (CreatureUtils.GetSavingThrow(caster, target, SaveAbility, spellDC) == SavingThrowResult.Failure)
      {
        target.ClearActionQueue();
        target.PlayVoiceChat(VoiceChatType.Laugh);
        NWScript.AssignCommand(target, () => _ = target.PlayAnimation(Animation.LoopingTalkLaughing, 1, duration: duration));
        Effect eff = Effect.LinkEffects(Effect.Knockdown(), Effect.VisualEffect(VfxType.DurMindAffectingDisabled),
          Effect.RunAction(onRemovedHandle: onRemoveFouRireDeTashaCallback, onIntervalHandle: onIntervalFouRireDeTashaCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = FouRireDeTashaEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Creator = caster;
        eff.IntParams[5] = (int)DCAbility;

        target.OnDamaged -= OnDamagedFouRire;
        target.OnDamaged += OnDamagedFouRire;

        target.ApplyEffect(EffectDuration.Temporary, eff, duration);
      }
      //}
    }
    private static ScriptHandleResult OnIntervalFouRire(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();
      var eff = eventData.Effect;

      if (eventData.EffectTarget is NwCreature target)
      {
        if (eff.Creator is NwCreature caster)
        {
          int spellDC = SpellUtils.GetCasterSpellDC(caster, (Ability)eff.IntParams[5]);

          if (CreatureUtils.GetSavingThrow(caster, target, Ability.Wisdom, spellDC) != SavingThrowResult.Failure)
            EffectUtils.RemoveTaggedEffect(target, FouRireDeTashaEffectTag);
        }
        else
          EffectUtils.RemoveTaggedEffect(target, FouRireDeTashaEffectTag);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveFouRire(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.OnDamaged -= OnDamagedFouRire;

      return ScriptHandleResult.Handled;
    }
    private static void OnDamagedFouRire(CreatureEvents.OnDamaged onDamaged)
    {
      var eff = onDamaged.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == FouRireDeTashaEffectTag);

      if(eff is not null)
      {
        if (eff.Creator is NwCreature caster)
        {
          int spellDC = SpellUtils.GetCasterSpellDC(caster, (Ability)eff.IntParams[5]);

          if (CreatureUtils.GetSavingThrow(caster, onDamaged.Creature, Ability.Wisdom, spellDC, Spells2da.spellTable[CustomSpell.FouRireDeTasha], SpellConfig.SpellEffectType.Tasha) != SavingThrowResult.Failure)
            EffectUtils.RemoveTaggedEffect(onDamaged.Creature, FouRireDeTashaEffectTag);
        }
        else
          EffectUtils.RemoveTaggedEffect(onDamaged.Creature, FouRireDeTashaEffectTag);
      }
      else
        onDamaged.Creature.OnDamaged -= OnDamagedFouRire;
    }
  }
}
