using System.Linq;
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
        Effect eff = Effect.RunAction(onIntervalHandle: onIntervalEntraveCallback, interval: NwTimeSpan.FromRounds(1));
        eff.Tag = EntraveEffectTag;
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
          var bonusAction = target.ActiveEffects.FirstOrDefault(e => e.Tag == BonusActionEffectTag);

          if (bonusAction is null)
            target.RemoveEffect(eventData.Effect);
          else
          {
            if (caster.IsCreatureSeen(target) && caster.DistanceSquared(target) < 325)
            {
              NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(Utils.Roll(12), DamageType.Electrical)));
              target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningS));
              target.RemoveEffect(bonusAction);
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

