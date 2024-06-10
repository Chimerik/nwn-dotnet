using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LienTotemOursAuraEffectTag = "_TOTEM_LIEN_OURS_AURA_EFFECT";
    private static ScriptCallbackHandle onEnterTotemLienOursCallback;
    private static ScriptCallbackHandle onExitTotemLienOursCallback;
    
    public static Effect totemLienOursAura
    {
      get
      {
        Effect eff = Effect.AreaOfEffect((PersistentVfxType)184, onEnterHandle: onEnterTotemLienOursCallback, onExitHandle: onExitTotemLienOursCallback);
        eff.Tag = LienTotemOursAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterTotemLlienOursAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1 || entering == protector
        || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, provocation));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitTotemLienOursAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, ProvocationEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
