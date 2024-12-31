using Anvil.API;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Enlarge(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        if (target.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).HasNothing)
          target.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value = target.VisualTransform.Scale;

        target.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Scale = oTarget.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value * 2; });
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.enlargeEffect, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }

      return targets;
    }
  }
}
