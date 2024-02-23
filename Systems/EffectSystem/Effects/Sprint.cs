using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SprintEffectTag = "_EFFECT_SPRINT";
    public const string SprintMobileEffectTag = "_EFFECT_SPRINT_MOBILE";
    private static ScriptCallbackHandle onRemoveSprintCallback;
    public static Effect sprintEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(50), Effect.Icon(NwGameTables.EffectIconTable.GetRow(142)),
          Effect.RunAction(onRemovedHandle: onRemoveSprintCallback));
        eff.Tag = SprintEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect sprintMobileEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Immunity(ImmunityType.Entangle), Effect.Immunity(ImmunityType.Slow));
        eff.Tag = SprintMobileEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveSprint(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      if (creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Chargeur)))
        creature.GetObjectVariable<LocalVariableLocation>("_CHARGER_INITIAL_LOCATION").Delete();

      return ScriptHandleResult.Handled;
    }
  }
}
