using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AgressionOrcEffectTag = "_AGRESSION_ORC_EFFECT";
    private static ScriptCallbackHandle onRemoveAgressionOrcCallback;
    public static Effect agressionOrc
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(50), Effect.RunAction(onRemovedHandle:onRemoveAgressionOrcCallback));
        eff.Tag = AgressionOrcEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveAgressionOrc(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.Commandable = true;

      return ScriptHandleResult.Handled;
    }
  }
}
