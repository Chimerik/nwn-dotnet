using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RepliqueInvoqueeAuraEffectTag = "_REPLIQUE_INVOQUEE_AURA_EFFECT";
    public const string RepliqueInvoqueeEffectTag = "_REPLIQUE_INVOQUEE_EFFECT";
    public static readonly Native.API.CExoString RepliqueInvoqueeEffectExoTag = RepliqueInvoqueeEffectTag.ToExoString();
    private static ScriptCallbackHandle onEnterRepliqueInvoqueeCallback;
    private static ScriptCallbackHandle onExitRepliqueInvoqueeCallback;
    private static ScriptCallbackHandle onRemoveRepliqueInvoqueeCallback;

    public static Effect GetRepliqueInvoqueeAuraEffect(NwCreature caster)
    {
      caster.OnDamaged -= OnDamageRemoveRempliqueInvoquee;
      caster.OnDamaged += OnDamageRemoveRempliqueInvoquee;

      Effect eff = Effect.LinkEffects(Effect.RunAction(onRemovedHandle: onRemoveRepliqueInvoqueeCallback), Effect.Concealment(50), Effect.VisualEffect(VfxType.DurGhostlyVisage),
        Effect.AreaOfEffect(PersistentVfxType.MobCircgood, onEnterHandle: onEnterRepliqueInvoqueeCallback, onExitHandle: onExitRepliqueInvoqueeCallback));
      eff.Tag = RepliqueInvoqueeAuraEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    public static Effect RepliqueInvoquee
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = RepliqueInvoqueeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterRepliqueInvoquee(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || entering == protector || !entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, RepliqueInvoquee));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitRepliqueInvoquee(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, RepliqueInvoqueeEffectTag);
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveRepliqueInvoquee(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.OnDamaged -= OnDamageRemoveRempliqueInvoquee;

      return ScriptHandleResult.Handled;
    }
    private static void OnDamageRemoveRempliqueInvoquee(CreatureEvents.OnDamaged onDmg)
    {
      EffectUtils.RemoveTaggedEffect(onDmg.Creature, RepliqueInvoqueeAuraEffectTag);
    }
  }
}
