using Anvil.API;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Enlarge(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if(oTarget.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).HasNothing)
        oTarget.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value = oTarget.VisualTransform.Scale;

      oTarget.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Scale = oTarget.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value * 2; });
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
      oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.enlargeEffect, NwTimeSpan.FromRounds(spellEntry.duration));

      if (oCaster.GetObjectVariable<LocalVariableInt>("_ENLARGE_DUERGAR").HasNothing)
      {
        if(oCaster is NwCreature caster)
        EffectSystem.ApplyConcentrationEffect(caster, spell.Id, new List<NwGameObject> { oTarget }, spellEntry.duration);
      }
      else
        oCaster.GetObjectVariable<LocalVariableInt>("_ENLARGE_DUERGAR").Delete();
    }
  }
}
