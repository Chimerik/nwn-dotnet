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
    public const string DuelForceEffectTag = "_DUEL_FORCE_EFFECT";
    public const string DuelForceCasterEffectTag = "_DUEL_FORCE_CASTER_EFFECT";
    private static ScriptCallbackHandle onRemoveDuelForceCallback;
    private static ScriptCallbackHandle onIntervalDuelForceCasterCallback;
    private static ScriptCallbackHandle onRemoveDuelForceCasterForceCallback;
    public static void ApplyDuelForce(NwCreature target, NwCreature caster, NwSpell spell, TimeSpan duration, Ability SaveAbility, Ability DCAbility)
    {
      int spellDC = SpellUtils.GetCasterSpellDC(caster, DCAbility);

      if (CreatureUtils.GetSavingThrowResult(target, SaveAbility, caster, spellDC) == SavingThrowResult.Failure)
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative),
          Effect.RunAction(onRemovedHandle: onRemoveDuelForceCallback));
        eff.Tag = DuelForceEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Creator = caster;
        eff.Spell = spell;

        target.MovementRate = MovementRate.Immobile;

        target.OnDamaged -= OnDamagedDuelForce;
        target.OnDamaged += OnDamagedDuelForce;

        target.ApplyEffect(EffectDuration.Temporary, eff, duration);

        Effect casterEffect = Effect.RunAction(onRemovedHandle: onRemoveDuelForceCasterForceCallback, onIntervalHandle: onIntervalDuelForceCasterCallback, interval: NwTimeSpan.FromRounds(1));
        eff.Tag = DuelForceCasterEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Creator = target;

        caster.OnCreatureAttack -= OnAttackDuelForceCaster;
        caster.OnCreatureAttack += OnAttackDuelForceCaster;
        caster.OnSpellAction -= OnSpellCastDuelForceCaster;
        caster.OnSpellAction += OnSpellCastDuelForceCaster;

        caster.ApplyEffect(EffectDuration.Temporary, casterEffect, duration);
      }
    }
    private static ScriptHandleResult OnIntervalDuelForceCaster(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();
      var eff = eventData.Effect;

      if (eventData.EffectTarget is NwCreature caster)
      {
        if (eff.Creator is NwCreature target)
        {
          if(caster.DistanceSquared(target) > 81)
          {
            EffectUtils.RemoveTaggedEffect(target, caster, DuelForceEffectTag);
            EffectUtils.RemoveTaggedEffect(caster, DuelForceCasterEffectTag);
          }
        }
        else
          EffectUtils.RemoveTaggedEffect(caster, DuelForceCasterEffectTag);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveDuelForce(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.OnDamaged -= OnDamagedDuelForce;
        creature.MovementRate = creature.IsLoginPlayerCharacter ? MovementRate.PC : MovementRate.CreatureDefault;

        if (eventData.Effect.Creator is NwCreature caster)
          EffectUtils.RemoveTaggedEffect(caster, DuelForceCasterEffectTag);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveDuelForceCaster(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
      {
        caster.OnCreatureAttack -= OnAttackDuelForceCaster;
        caster.OnSpellAction -= OnSpellCastDuelForceCaster;

        if (eventData.Effect.Creator is NwCreature target)
          EffectUtils.RemoveTaggedEffect(target, caster, DuelForceEffectTag);
      }

      return ScriptHandleResult.Handled;
    }
    private static void OnDamagedDuelForce(CreatureEvents.OnDamaged onDamaged)
    {
      var eff = onDamaged.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == DuelForceEffectTag);

      if (eff is not null)
      {
        if (eff.Creator is NwCreature caster)
        {
          var oDamager = NWScript.GetLastDamager(onDamaged.Creature).ToNwObject<NwObject>();

          if(oDamager != caster)
          {
            EffectUtils.RemoveTaggedEffect(onDamaged.Creature, caster, DuelForceEffectTag);
            EffectUtils.RemoveTaggedEffect(caster, DuelForceCasterEffectTag);
          }
        }
        else
          EffectUtils.RemoveTaggedEffect(onDamaged.Creature, DuelForceEffectTag);
      }
      else
        onDamaged.Creature.OnDamaged -= OnDamagedFouRire;
    }
    private static void OnAttackDuelForceCaster(OnCreatureAttack onAttack)
    {
      var eff = onAttack.Attacker.ActiveEffects.FirstOrDefault(e => e.Tag == DuelForceCasterEffectTag);

      if (eff is not null)
      {
        if (eff.Creator is NwCreature target)
        {
          if (target != onAttack.Target)
          {
            EffectUtils.RemoveTaggedEffect(target, onAttack.Attacker, DuelForceEffectTag);
            EffectUtils.RemoveTaggedEffect(onAttack.Attacker, DuelForceCasterEffectTag);
          }
        }
        else
          EffectUtils.RemoveTaggedEffect(onAttack.Attacker, DuelForceCasterEffectTag);
      }
    }
    private static void OnSpellCastDuelForceCaster(OnSpellAction onSpellAction)
    {
      if (onSpellAction.TargetObject is not null)
      {
        var eff = onSpellAction.Caster.ActiveEffects.FirstOrDefault(e => e.Tag == DuelForceCasterEffectTag);

        if (eff is not null)
        {
          if (eff.Creator is NwCreature target)
          {
            if (target != onSpellAction.TargetObject)
            {
              EffectUtils.RemoveTaggedEffect(target, onSpellAction.Caster, DuelForceEffectTag);
              EffectUtils.RemoveTaggedEffect(onSpellAction.Caster, DuelForceCasterEffectTag);
            }
          }
          else
            EffectUtils.RemoveTaggedEffect(onSpellAction.Caster, DuelForceCasterEffectTag);
        }
      }
    }
  }
}
