using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RecklessAttackEffectTag = "_RECKLESS_ATTACK_EFFECT";
    public static readonly Native.API.CExoString RecklessAttackEffectExoTag = "_RECKLESS_ATTACK_EFFECT".ToExoString();
    private static ScriptCallbackHandle onRemoveRecklessAttackCallback;
    public static Effect RecklessAttackEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.ACDecrease), Effect.Icon(EffectIcon.AttackIncrease), 
          Effect.RunAction(onRemovedHandle: onRemoveRecklessAttackCallback));
        eff.Tag = RecklessAttackEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveRecklessAttack(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.SetFeatRemainingUses((Feat)CustomSkill.BarbarianRecklessAttack, 1);

      return ScriptHandleResult.Handled;
    }
  }
}
