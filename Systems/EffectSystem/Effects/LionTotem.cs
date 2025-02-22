using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LionTotemEffectTag = "_LION_TOTEM_EFFECT";
    private static ScriptCallbackHandle onEnterLionTotemCallback;
    private static ScriptCallbackHandle onExitLionTotemCallback;
    
    public static Effect LionTotem
    {
      get
      {
        Effect eff = Effect.AreaOfEffect(PersistentVfxType.MobTideOfBattle, onEnterHandle: onEnterLionTotemCallback, onExitHandle: onExitLionTotemCallback);
        eff.Tag = LionTotemEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterLionTotem(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || entering == protector
        || !entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      ApplyProvocation(protector, entering, TimeSpan.Zero);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitLionTotem(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, ProvocationEffectTag);
      EffectUtils.RemoveTaggedEffect(protector, exiting, ProvocationEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
