using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EnlargeEffectTag = "_EFFECT_ENLARGE";
    public static Effect enlargeEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.DamageIncrease(2, DamageType.BaseWeapon), Effect.VisualEffect(VfxType.DurCessatePositive));
        eff.Tag = EnlargeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static void OnRemoveEnlarge(OnEffectRemove onEffect)
    {
      if (!onEffect.Effect.IsValid || onEffect.Effect.Tag != EnlargeEffectTag || onEffect.Object is not NwGameObject target)
        return;

      target.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Scale = target.GetObjectVariable<PersistentVariableFloat>("_ORIGINAL_SIZE").Value; });
    }
  }
}
