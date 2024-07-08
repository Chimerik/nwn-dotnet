using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HaloDeLumiereAuraEffectTag = "_HALO_DE_LUMIERE_AURA_EFFECT";
    public const string HaloDeLumiereEffectTag = "_HALO_DE_LUMIERE_EFFECT";
    private static ScriptCallbackHandle onEnterHaloDeLumiereCallback;
    private static ScriptCallbackHandle onExitHaloDeLumiereCallback;

    public static Effect HaloDeLumiereAura
    {
      get
      {
        Effect eff = Effect.AreaOfEffect((PersistentVfxType)60, onEnterHandle: onEnterHaloDeLumiereCallback, onExitHandle: onExitHaloDeLumiereCallback);
        eff.Tag = HaloDeLumiereAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect HaloDeLumiere
    {
      get
      {
        Effect eff = Effect.SavingThrowDecrease(SavingThrow.All, 1, SavingThrowType.Spell);
        eff.Tag = HaloDeLumiereEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterHaloDeLumiere(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1
        || !entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, HaloDeLumiere));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitHaloDeLumiere(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, HaloDeLumiereEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
