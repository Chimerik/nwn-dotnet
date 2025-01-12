using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProtectionStyleAuraEffectTag = "_PROTECTION_STYLE_AURA_EFFECT";
    public const string ProtectionStyleEffectTag = "_PROTECTION_STYLE_EFFECT";
    private static ScriptCallbackHandle onEnterProtectionStyleCallback;
    private static ScriptCallbackHandle onExitProtectionStyleCallback;
    
    public static Effect protectionStyleAura
    {
      get
      {
        Effect eff = Effect.AreaOfEffect((PersistentVfxType)184, onEnterHandle: onEnterProtectionStyleCallback, onExitHandle: onExitProtectionStyleCallback);
        eff.Tag = ProtectionStyleAuraEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public static Effect protectionStyle
    {
      get
      {
        Effect eff = Effect.Icon((EffectIcon)153);
        eff.Tag = ProtectionStyleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterProtectionStyle(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1 || entering == protector
        || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, protectionStyle));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitProtectionStyle(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, ProtectionStyleEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
