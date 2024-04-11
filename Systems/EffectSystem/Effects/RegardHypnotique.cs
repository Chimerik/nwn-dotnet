using System;
using System.Linq;
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
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.Paralyze), Effect.CutsceneParalyze(), Effect.RunAction(onIntervalHandle: onIntervalRegardHypnotiqueCallback, interval: TimeSpan.FromSeconds(1)));
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

      if (eventData.Effect.Creator is not NwCreature caster || target.DistanceSquared(caster) > 4)
      {
        target.RemoveEffect(eventData.Effect);
        target.OnDamaged -= OnDamageRegardHypnotique;
        return ScriptHandleResult.Handled;
      }

      return ScriptHandleResult.Handled;
    }
    public static void OnDamageRegardHypnotique(CreatureEvents.OnDamaged onDamage)
    {
      foreach (var eff in onDamage.Creature.ActiveEffects.Where(e => e.Tag == RegardHypnotiqueEffectTag))
        onDamage.Creature.RemoveEffect(eff);
    }
  }
}
