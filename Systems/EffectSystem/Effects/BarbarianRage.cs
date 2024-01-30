using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveBarbarianRageCallback;
    private static ScriptCallbackHandle onIntervalBarbarianRageCallback;
    public const string BarbarianRageEffectTag = "_EFFECT_BARBARIAN_RAGE";
    public static readonly Native.API.CExoString barbarianRageEffectExoTag = "_EFFECT_BARBARIAN_RAGE".ToExoString();
    public static Effect barbarianRageEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurCessatePositive), Effect.RunAction(onRemovedHandle: onRemoveBarbarianRageCallback, onIntervalHandle: onIntervalBarbarianRageCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = BarbarianRageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveBarbarianRage(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      target.OnCreatureAttack -= CreatureUtils.OnAttackBarbarianRage;
      target.OnDamaged -= CreatureUtils.OnDamagedBarbarianRage;
      target.OnItemEquip -= ItemSystem.OnEquipBarbarianRage;
      target.OnSpellAction -= SpellSystem.CancelSpellBarbarianRage;
      target.OnDamaged -= CreatureUtils.OnDamagedRageImplacable;

      if (target.KnowsFeat(NwFeat.FromFeatId(CustomSkill.BersekerFrenziedStrike)))
      {
        target.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.BersekerFrenziedStrike), 0);
        target.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeFrenetiqueMalusVariable).Delete();
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnIntervalBarbarianRage(CallInfo callInfo)
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
