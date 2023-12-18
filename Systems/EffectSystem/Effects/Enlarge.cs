using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveEnlargeCallback;
    public const string EnlargeEffectTag = "_EFFECT_ENLARGE";
    public static Effect enlargeEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.DamageIncrease(2, DamageType.BaseWeapon), 
          Effect.VisualEffect(VfxType.DurCessatePositive), Effect.RunAction(onRemovedHandle: onRemoveEnlargeCallback));
        eff.Tag = EnlargeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveEnlarge(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwGameObject target)
        return ScriptHandleResult.Handled;

      target.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Scale = target.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value; });

      return ScriptHandleResult.Handled;
    }
  }
}
