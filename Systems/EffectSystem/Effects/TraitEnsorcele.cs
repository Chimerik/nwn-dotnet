﻿using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TraitEnsorceleEffectTag = "_TRAIT_ENSORCELE_EFFECT";
    private static ScriptCallbackHandle onIntervalTraitEnsorceleCallback;
    public static Effect TraitEnsorcele
    {
      get
      {
        Effect eff = Effect.RunAction(onIntervalHandle: onIntervalEntraveCallback, interval: NwTimeSpan.FromRounds(1));
        eff.Tag = EntraveEffectTag;
        eff.SubType = EffectSubType.Supernatural;

        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalTraitEnsorcele(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
      {
        if (eventData.Effect.Creator is NwCreature caster)
        {
          if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value > 0)
          {
            if (caster.IsCreatureSeen(target) && caster.DistanceSquared(target) < 325)
            {
              NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(Utils.Roll(12), DamageType.Electrical)));
              target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningS));
              caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;
            }
            else
              target.RemoveEffect(eventData.Effect);
          }
        }
        else
          target.RemoveEffect(eventData.Effect);
      }

      return ScriptHandleResult.Handled;
    }
  }
}

