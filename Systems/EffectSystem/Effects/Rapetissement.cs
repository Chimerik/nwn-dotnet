﻿using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveRapetissementCallback;
    public const string RapetissementEffectTag = "_EFFECT_RAPETISSEMENT";
    public static Effect Rapetissement(NwGameObject target)
    {
      if (target.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).HasNothing)
        target.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value = target.VisualTransform.Scale;

      target.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Scale = target.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value / 2; });
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReduceAbilityScore));

      Effect eff = Effect.RunAction(onRemovedHandle: onRemoveEnlargeCallback);
      eff.Tag = EnlargeEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}