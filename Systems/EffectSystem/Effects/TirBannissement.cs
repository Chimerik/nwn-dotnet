using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveTirBannissementCallback;
    public const string tirBannissementEffectTag = "_TIR_BANNISSEMENT_EFFECT";
    public static Effect tirBannissementEffect
    {
      get
      {
        Effect eff = Effect.RunAction(onRemovedHandle: onRemoveTirBannissementCallback);
        eff.Tag = tirBannissementEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveTirBannissement(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwGameObject target)
        return ScriptHandleResult.Handled;

      target.VisibilityOverride = VisibilityMode.Default;
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));

      return ScriptHandleResult.Handled;
    }
  }
}
