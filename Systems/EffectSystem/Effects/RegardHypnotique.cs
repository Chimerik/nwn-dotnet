using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalRegardHypnotiqueCallback;
    public const string RegardHypnotiqueEffectTag = "_REGARD_HYPNOTIQUE_EFFECT";
    public static Effect RegardHypnotique
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Paralyze(), Effect.RunAction(onIntervalHandle: onIntervalRegardHypnotiqueCallback, interval: TimeSpan.FromSeconds(1)));
        eff.Tag = RegardHypnotiqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalRegardHypnotique(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if(eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is NwCreature caster && target.DistanceSquared(caster) > 16)
      {
        target.RemoveEffect(eventData.Effect);
        target.OnDamaged -= OnDamageRegardHypnotique;
        return ScriptHandleResult.Handled;
      }

      return ScriptHandleResult.Handled;
    }
    public static void OnDamageRegardHypnotique(CreatureEvents.OnDamaged onDamage)
    {
      EffectUtils.RemoveTaggedEffect(onDamage.Creature, RegardHypnotiqueEffectTag);
    }
  }
}
