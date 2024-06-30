using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveBannissementCallback;
    public const string BannissementEffectTag = "_BANNISSEMENT_EFFECT";
    public static Effect GetBannissementEffect(NwCreature target)
    {
      target.VisibilityOverride = VisibilityMode.Hidden;
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
      target.PlotFlag = true;

      Effect eff = Effect.LinkEffects(Effect.CutsceneParalyze(), Effect.RunAction(onRemovedHandle: onRemoveBannissementCallback));
      eff.Tag = BannissementEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult OnRemoveBannissement(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      target.VisibilityOverride = VisibilityMode.Default;
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      target.PlotFlag = false;

      return ScriptHandleResult.Handled;
    }
  }
}
