using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static ScriptHandleResult OnEnterThreatRange(CallInfo callInfo)
    {
      var eventData = new AreaOfEffectEvents.OnEnter();

      if (eventData.Entering is NwCreature entering && eventData.Effect.Creator is NwCreature threatOrigin && entering.IsReactionTypeHostile(threatOrigin))
        entering.ApplyEffect(EffectDuration.Permanent, threatenedEffect);

      return ScriptHandleResult.Handled;
    }
    public static ScriptHandleResult OnExitThreatRange(CallInfo callInfo)
    {
      var eventData = new AreaOfEffectEvents.OnExit();
      eventData.Exiting.RemoveEffect(threatenedEffect);

      return ScriptHandleResult.Handled;
    }
  }
}
