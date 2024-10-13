using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ElkAspectAuraEffectTag = "_ELK_ASPECT_AURA_EFFECT";
    public const string ElkAspectEffectTag = "_ELK_ASPECT_EFFECT";
    private static ScriptCallbackHandle onEnterElkAspectAuraCallback;
    private static ScriptCallbackHandle onExitElkAspectAuraCallback;
    
    public static Effect elkAspectAura
    {
      get
      {
        Effect eff = Effect.AreaOfEffect((PersistentVfxType)193, onEnterHandle: onEnterElkAspectAuraCallback, onExitHandle: onExitElkAspectAuraCallback);
        eff.Tag = ElkAspectAuraEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public static Effect elkAspect
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(5);
        eff.Tag = ElkAspectEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterElkAspectAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || entering == protector || protector.HP < 1
        || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, elkAspect));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitElkAspectAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, ElkAspectEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
