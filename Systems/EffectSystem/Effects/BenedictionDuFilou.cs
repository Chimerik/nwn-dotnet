using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BenedictionDuFilouEffectTag = "_BENEDICTION_DU_FILOU_EFFECT";
    public const string CurrentBenedictionDuFilouTargetVariable = "_CURRENT_BENEDICTION_DU_FILOU_TARGET";
    public static Effect BenedictionDuFilou(NwCreature caster, NwCreature target)
    {
      EffectUtils.RemoveTaggedEffect(target, BenedictionDuFilouEffectTag);
      NwCreature previousTarget = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CurrentBenedictionDuFilouTargetVariable).Value;

      if (previousTarget is not null && previousTarget.IsValid)
      {
        EffectUtils.RemoveTaggedEffect(previousTarget, BenedictionDuFilouEffectTag);
        previousTarget.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect(VfxType.DurCessatePositive), TimeSpan.FromSeconds(0.1));
      }

      if (target.IsPlayerControlled)
      {
        target.ControllingPlayer.OnClientDisconnect -= OnClientDisconnectBenedictionDuFilou;
        target.ControllingPlayer.OnClientDisconnect += OnClientDisconnectBenedictionDuFilou;
      }

      caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CurrentBenedictionDuFilouTargetVariable).Value = target;

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));

      Effect eff = Effect.Icon(CustomEffectIcon.BenedictionDuFilou);
      eff.Tag = BenedictionDuFilouEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static void OnClientDisconnectBenedictionDuFilou(OnClientDisconnect onPCDisconnect)
    {
      EffectUtils.RemoveTaggedEffect(onPCDisconnect.Player.LoginCreature, BenedictionDuFilouEffectTag);
      onPCDisconnect.Player.OnClientDisconnect -= OnClientDisconnectBenedictionDuFilou;
    }
  }
}
