using System.Linq;
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
      Effect damageReduction = Effect.DamageReduction(intensity, DamagePower.Plus20);
      damageReduction.ShowIcon = false;
      Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.DamageReduction), damageReduction, Effect.RunAction(onRemovedHandle: onRemoveAbjurationWardCallback));
      eff.CasterLevel = intensity;
      eff.Tag = AbjurationWardEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      LogUtils.LogMessage($"Protection arcanique : intensité {intensity}", LogUtils.LogType.Combat);
      return eff;
    }
    private static ScriptHandleResult OnRemoveAbjurationWard(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target || target.GetObjectVariable<LocalVariableInt>(CreatureUtils.AbjurationWardForcedTriggerVariable).HasNothing)
        return ScriptHandleResult.Handled;

      var ward = eventData.Effect;
      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.AbjurationWardForcedTriggerVariable).Delete();

      AbjurationWardIntensityReduction(ward, target);
      return ScriptHandleResult.Handled;
    }
    public static void AbjurationWardIntensityReduction(Effect ward, NwCreature target)
    {
      if (ward.Creator != target)
      {
        if (ward.Creator is not NwCreature creator || !creator.IsValid)
        {
          EffectUtils.RemoveTaggedEffect(target, AbjurationWardEffectTag);
          return;
        }
        else
        {
          float distance = target.DistanceSquared(creator);

          if (!(0 < distance && distance < 81))
          {
            WizardUtils.ResetAbjurationWard(creator);
            LogUtils.LogMessage($"{target.Name} - Dégâts réduits par protection arcanique", LogUtils.LogType.Combat);
            LogUtils.LogMessage($"{target.Name} - Protection Arcanique dissipée en raison de la distance et réappliquée à {creator.Name}", LogUtils.LogType.Combat);

            return;
          }
        }
      }

      EffectUtils.RemoveTaggedEffect(target, ward.Creator, AbjurationWardEffectTag);

      if (ward.CasterLevel > 1)
        NWScript.AssignCommand(ward.Creator, () => target.ApplyEffect(EffectDuration.Permanent, GetAbjurationWardEffect(ward.CasterLevel - 1)));

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));

      LogUtils.LogMessage($"{target.Name} - Dégâts réduits par protection arcanique", LogUtils.LogType.Combat);
    }
    public static void OnDeathAbjurationWard(CreatureEvents.OnDeath death)
    {
      NwCreature target = death.KilledCreature;
      var ward = target.ActiveEffects.FirstOrDefault(e => e.Tag == AbjurationWardEffectTag);

      if(ward is null || ward.Creator is not NwCreature caster)
      {
        target.OnDeath -= OnDeathAbjurationWard;
        return;
      }

      WizardUtils.ResetAbjurationWard(caster);
    }
    public static void OnLeaveAbjurationWard(OnClientDisconnect disco)
    {
      NwCreature target = disco.Player.LoginCreature;
      var ward = target.ActiveEffects.FirstOrDefault(e => e.Tag == AbjurationWardEffectTag);

      if (ward is null || ward.Creator is not NwCreature caster)
      {
        disco.Player.OnClientDisconnect -= OnLeaveAbjurationWard;
        return;
      }

      WizardUtils.ResetAbjurationWard(caster);
    }
  }
}
