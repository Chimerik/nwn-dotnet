using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ContreCharmeAuraEffectTag = "_CONTRE_CHARME_AURA_EFFECT";
    public const string ContreCharmeEffectTag = "_CONTRE_CHARME_EFFECT";

    private static ScriptCallbackHandle onEnterContreCharmeCallback;
    private static ScriptCallbackHandle onExitContreCharmeCallback;
    
    public static Effect ContreCharmeAura
    {
      get
      {
        Effect eff = Effect.AreaOfEffect((PersistentVfxType)191, onEnterHandle: onEnterContreCharmeCallback, onExitHandle: onExitContreCharmeCallback);
        eff.Tag = ContreCharmeAuraEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public static Effect ContreCharme
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.ImmunityCharm), Effect.Icon(EffectIcon.ImmunityFear));
        eff.Tag = ContreCharmeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterContreCharme(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1 || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, ContreCharme));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitContreCharme(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, ProtectionStyleEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
