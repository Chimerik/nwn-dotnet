using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AbjurationWardEffectTag = "_ABJURATION_WARD_EFFECT";
    public static readonly Native.API.CExoString abjurationWardEffectExoTag = AbjurationWardEffectTag.ToExoString();
    private static ScriptCallbackHandle onRemoveAbjurationWardCallback;
    public static Effect GetAbjurationWardEffect(int intensity)
    {
      
      Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.DamageReduction), Effect.DamageReduction(intensity, DamagePower.Plus20), 
        Effect.RunAction(onRemovedHandle: onRemoveAbjurationWardCallback));
      eff.CasterLevel = intensity;
      eff.Tag = AbjurationWardEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult OnRemoveAbjurationWard(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target || target.GetObjectVariable<LocalVariableInt>(CreatureUtils.AbjurationWardForcedTriggerVariable).HasNothing)
        return ScriptHandleResult.Handled;

      var ward = eventData.Effect;
      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.AbjurationWardForcedTriggerVariable).Delete();

      if (ward.Creator != target)
      {
        if (ward.Creator is not NwCreature creator || !creator.IsValid)
        {
          EffectUtils.RemoveTaggedEffect(target, AbjurationWardEffectTag);
          return ScriptHandleResult.Handled;
        }
        else if (creator.Area != target.Area || target.DistanceSquared(creator) > 80)
        {
          EffectUtils.RemoveTaggedEffect(target, creator, AbjurationWardEffectTag);
          NWScript.AssignCommand(creator, () => creator.ApplyEffect(EffectDuration.Permanent, GetAbjurationWardEffect(ward.CasterLevel)));
        }

        target.OnDamaged -= WizardUtils.OnDamageAbjurationWard;
      }

      EffectUtils.RemoveTaggedEffect(target, ward.Creator, AbjurationWardEffectTag);

      if (ward.CasterLevel > 1)
        NWScript.AssignCommand(ward.Creator, () => target.ApplyEffect(EffectDuration.Permanent, GetAbjurationWardEffect(ward.CasterLevel - 1)));

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));

      LogUtils.LogMessage($"{target.Name} - Dégâts réduits par protection arcanique", LogUtils.LogType.Combat);

      return ScriptHandleResult.Handled;
    }
  }
}
