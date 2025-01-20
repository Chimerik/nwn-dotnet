using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PassageSansTraceEffectTag = "_PASSAGE_SANS_TRACE_AURA_EFFECT";
    public const string PassageSansTraceBonusEffectTag = "_PASSAGE_SANS_TRACE_BONUS_EFFECT";
    private static ScriptCallbackHandle onEnterPassageSansTraceCallback;
    private static ScriptCallbackHandle onExitPassageSansTraceCallback;

    public static Effect PassageSansTrace(NwGameObject caster)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.MobDragonFear, onEnterHandle: onEnterPassageSansTraceCallback, onExitHandle: onExitPassageSansTraceCallback);
      eff.Tag = PassageSansTraceEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;

      return eff;
    }
    private static Effect PassageSansTraceBonus
    {
      get
      {
        Effect eff = Effect.SkillIncrease(NwSkill.FromSkillType(Skill.MoveSilently), 10);
        eff.Tag = PassageSansTraceBonusEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterPassageSansTrace(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || entering.IsReactionTypeHostile(protector) 
        || entering.ActiveEffects.Any(e => e.Tag == PassageSansTraceBonusEffectTag))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, PassageSansTraceBonus));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitPassageSansTrace(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, PassageSansTraceBonusEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
