using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HateEffectTag = "_HATE_EFFECT";
    private static ScriptCallbackHandle onRemoveHateCallback;
    public static Effect Hate
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Haste(), Effect.RunAction(onRemovedHandle:onRemoveHateCallback));
        eff.Tag = HateEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveHate(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.ApplyEffect(EffectDuration.Temporary, Effect.CutsceneParalyze(), NwTimeSpan.FromRounds(1));

      return ScriptHandleResult.Handled;
    }
  }
}
