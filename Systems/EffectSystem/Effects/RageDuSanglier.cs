using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveRageDuSanglierCallback;
    private static ScriptCallbackHandle onIntervalRageDuSanglierCallback;
    public const string RageDuSanglierEffectTag = "_RAGE_DU_SANGLIER_EFFECT";
    public static readonly Native.API.CExoString rageDuSanglierEffectExoTag = RageDuSanglierEffectTag.ToExoString();
    public static Effect RageDuSanglier
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)168), 
          Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50), Effect.DamageImmunityIncrease(DamageType.Piercing, 50), Effect.DamageImmunityIncrease(DamageType.Slashing, 50),
          Effect.RunAction(onRemovedHandle: onRemoveRageDuSanglierCallback, onIntervalHandle: onIntervalRageDuSanglierCallback, interval: NwTimeSpan.FromRounds(1)),
          Effect.ModifyAttacks(1));
        eff.Tag = RageDuSanglierEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveRageDuSanglier(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));

      target.OnCreatureAttack -= CreatureUtils.OnAttackBarbarianRage;
      target.OnDamaged -= CreatureUtils.OnDamagedBarbarianRage;

      target.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRageSanglier, 100);
     
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnIntervalRageDuSanglier(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if (target.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").HasNothing)
        EffectUtils.RemoveTaggedEffect(target, BarbarianRageEffectTag);
      else
        target.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Delete();

      return ScriptHandleResult.Handled;
    }
  }
}
