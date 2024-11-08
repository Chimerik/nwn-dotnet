using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string WolfTotemAuraEffectTag = "_WOLF_TOTEM_AURA_EFFECT";
    public const string WolfTotemEffectTag = "_WOLF_TOTEM_EFFECT";
    public static readonly Native.API.CExoString WolfTotemEffectExoTag = WolfTotemEffectTag.ToExoString();
    private static ScriptCallbackHandle onEnterWolfTotemAuraCallback;
    private static ScriptCallbackHandle onExitWolfTotemAuraCallback;
    
    public static Effect wolfTotemAura
    {
      get
      {
        Effect eff = Effect.AreaOfEffect(PersistentVfxType.MobTideOfBattle, onEnterHandle: onEnterWolfTotemAuraCallback, onExitHandle: onExitWolfTotemAuraCallback);
        eff.Tag = WolfTotemAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect wolfTotem
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = WolfTotemEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterWolfTotemAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || entering == protector
        || !entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, wolfTotem));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitWolfTotemAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, WolfTotemEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
