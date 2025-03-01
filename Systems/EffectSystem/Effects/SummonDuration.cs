using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SummonDurationEffectTag = "_SUMMON_DURATION_EFFECT";
    private static ScriptCallbackHandle onRemoveSummonDurationCallback;
    public static Effect SummonDuration
    {
      get
      {
        Effect eff = Effect.RunAction(onRemovedHandle: onRemoveSummonDurationCallback);
        eff.Tag = SummonDurationEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }

    private static ScriptHandleResult OnRemoveSummonDuration(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.Unsummon();
      }

      return ScriptHandleResult.Handled;
    }
  }
}
