using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EnduranceImplacableEffectTag = "_HALFORC_ENDURANCE_EFFECT";
    public const string EnduranceImplacableVariable = "_HALFORC_ENDURANCE";
    private static ScriptCallbackHandle onRemoveEnduranceImplacableCallback;

    public static Effect enduranceImplacable
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.EnduranceImplacable), Effect.RunAction(onRemovedHandle: onRemoveEnduranceImplacableCallback));
        eff.Tag = EnduranceImplacableEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }

    private static ScriptHandleResult OnRemoveEnduranceImplacable(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.Immortal = false;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
