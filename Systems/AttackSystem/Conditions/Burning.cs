using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static int GetBurningModifiedDuration(NwGameObject targetObject, int duration)
    {
      if (targetObject is not NwCreature targetCreature)
        return 0;

      bool applyBurning = true;

      foreach (var eff in targetCreature.ActiveEffects)
      {
        if (eff.Tag == "CUSTOM_CONDITION_BURNING")
        {
          if (eff.DurationRemaining > duration)
            applyBurning = false;
          else
            targetCreature.RemoveEffect(eff);
        }
      }

      return applyBurning ? duration : 0;
    }
    public static ScriptHandleResult ApplyBurning(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect((VfxType)115));

      /*if(PlayerSystem.Players.TryGetValue(oTarget, out PlayerSystem.Player player))
      {

      }
      else 
      {

      }*/

      return ScriptHandleResult.Handled;
    }
    public static ScriptHandleResult IntervalBurning(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect((VfxType)491)); // TODO : trouver un effet visuel pour la brûlure

      return ScriptHandleResult.Handled;
    }
  }
}
