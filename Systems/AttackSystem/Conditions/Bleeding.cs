using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static ScriptHandleResult ApplyBleeding(CallInfo _)
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
    public static ScriptHandleResult IntervalBleeding(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect((VfxType)115));

      return ScriptHandleResult.Handled;
    }
  }
}
