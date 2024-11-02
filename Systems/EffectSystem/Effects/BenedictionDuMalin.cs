using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BenedictionDuMalinAuraEffectTag = "_BENEDICTION_DU_MALIN_EFFECT";
    public const string BenedictionDuMalinEffectTag = "_BENEDICTION_DU_MALIN_EFFECT";
    private static ScriptCallbackHandle onEnterBenedictionDuMalinCallback;
    private static ScriptCallbackHandle onExitBenedictionDuMalinCallback;
    private static ScriptCallbackHandle onRemoveBenedictionDuMalinCallback;
    public static Effect BenedictionDuMalinAura(NwCreature caster)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterBenedictionDuMalinCallback, onExitHandle: onExitBenedictionDuMalinCallback);
      eff.Tag = BenedictionDuMalinAuraEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;
      return eff;
    }
    public static Effect BenedictionDuMalin
    {
      get
      {
        Effect eff = Effect.RunAction(onRemovedHandle: onRemoveBenedictionDuMalinCallback);
        eff.Tag = BenedictionDuMalinEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterBenedictionDuMalin(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1
        || !protector.IsReactionTypeHostile(entering))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, BenedictionDuMalin));

      entering.OnDeath -= OccultisteUtils.OnDeathBenedictionDuMalin;
      entering.OnDeath += OccultisteUtils.OnDeathBenedictionDuMalin;

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitBenedictionDuMalin(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector || !protector.IsReactionTypeHostile(exiting))
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, BenedictionDuMalinEffectTag);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onRemoveBenedictionDuMalin(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
      {
        caster.OnDeath -= OccultisteUtils.OnDeathBenedictionDuMalin;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
