using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AuraDeCourageEffectTag = "_AURA_DE_COURAGE_EFFECT";
    public const string CourageEffectTag = "_COURAGE_EFFECT";
    private static ScriptCallbackHandle onEnterAuraDeCourageCallback;
    private static ScriptCallbackHandle onExitAuraDeCourageCallback;
    public static Effect AuraDeCourage(NwCreature caster, int paladinLevel)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpAuraHoly, fScale: paladinLevel < 18 ? 1 : 2), 
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterAuraDeCourageCallback, onExitHandle: onExitAuraDeCourageCallback));
      eff.Tag = AuraDeCourageEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;
      return eff;
    }
    public static Effect Courage
    {
      get
      {
        Effect eff = Effect.Immunity(ImmunityType.Fear);
        eff.Tag = CourageEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterCourageAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1
        || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, Courage));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitCourageAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector || exiting.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, CourageEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
