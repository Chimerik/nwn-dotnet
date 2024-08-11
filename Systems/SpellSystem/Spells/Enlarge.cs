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

      if(oTarget.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).HasNothing)
        oTarget.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value = oTarget.VisualTransform.Scale;

      oTarget.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Scale = oTarget.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value * 2; });
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
      oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.enlargeEffect, SpellUtils.GetSpellDuration(oCaster, spellEntry));

      return new List<NwGameObject>() { oTarget };
    }
  }
}
