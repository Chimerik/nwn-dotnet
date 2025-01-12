using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ViseeStableEffectTag = "_VISEE_STABLE_EFFECT";
    public const string ViseeStableMalusEffectTag = "_VISEE_STABLE_MALUS_EFFECT";
    private static ScriptCallbackHandle onRemoveViseeStableCallback;
    public static void ApplyViseeStable(NwCreature caster)
    {
      Effect eff = Effect.Icon(EffectIcon.AttackIncrease);
      eff.Tag = ViseeStableEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      if (!caster.KnowsFeat((Feat)CustomSkill.AssassinInfiltrationExpert))
      {
        caster.MovementRate = MovementRate.Immobile;
        caster.ApplyEffect(EffectDuration.Temporary, ViseeStableMalus, NwTimeSpan.FromRounds(2));
      }

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));
      caster.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(2));
      
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Visée Stable", StringUtils.gold, true, true);
    }
    public static Effect ViseeStableMalus
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.MovementSpeedIncrease),
          Effect.RunAction(onRemovedHandle: onRemoveViseeStableCallback));
        eff.Tag = ViseeStableMalusEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveViseeStable(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.MovementRate = MovementRate.PC;

      return ScriptHandleResult.Handled;
    }
  }
}

