using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VolEffectTag = "_VOL_EFFECT";
    private static ScriptCallbackHandle onRemoveVolCallback;
    public static Effect Vol(NwGameObject caster, NwSpell spell = null)
    {
      caster.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Translation = new System.Numerics.Vector3(0, 0, 1); });

      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Vol), Effect.VisualEffect(CustomVfx.Vol, fScale: 0.4f),
        Effect.RunAction(onRemovedHandle: onRemoveVolCallback));
      eff.Tag = VolEffectTag;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }

    private static ScriptHandleResult OnRemoveVol(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
        creature.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Translation = new System.Numerics.Vector3(0, 0, 0); });

      return ScriptHandleResult.Handled;
    }
  }
}

