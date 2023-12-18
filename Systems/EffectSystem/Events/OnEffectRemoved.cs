using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnEffectRemoved(OnEffectRemove onEffect)
    {
      if (!onEffect.Effect.IsValid || onEffect.Object is not NwCreature target)
        return;

      /*switch (onEffect.Effect.Tag)
      {
        case ConcentrationEffectTag: OnRemoveConcentration(onEffect); return;
        case EnlargeEffectTag: target.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Scale = target.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value; }); return;
        case faerieFireEffectTag: target.OnEffectApply -= CheckFaerieFire; return;
        case boneChillEffectTag: target.OnHeal -= SpellSystem.PreventHeal; return;
      }*/

      OnRecoveryAddThreatRange(target, onEffect.Effect.EffectType);
    }
  }
}
