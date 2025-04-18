﻿using System.Linq;
using Anvil.API;
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
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.TraitEnsorcele),
          Effect.RunAction(onIntervalHandle: onIntervalTraitEnsorceleCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = TraitEnsorceleEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.TraitEnsorcele);
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
          var bonusAction = caster.ActiveEffects.FirstOrDefault(e => e.Tag == BonusActionEffectTag);

          if (bonusAction is not null)
          {
            if (caster.IsCreatureSeen(target) && caster.DistanceSquared(target) < 325)
            {
              NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(Utils.Roll(12), DamageType.Electrical)));
              target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningS));
              target.RemoveEffect(bonusAction);
            }
            else
            {
              SpellUtils.DispelConcentrationEffects(caster);
            }
          }
        }
        else
          target.RemoveEffect(eventData.Effect);
      }

      return ScriptHandleResult.Handled;
    }
  }
}

