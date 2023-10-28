using Anvil.API;
using Anvil.API.Events;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Enlarge(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);

      if(onSpellCast.TargetObject.GetObjectVariable<PersistentVariableFloat>("_ORIGINAL_SIZE").HasNothing)
        onSpellCast.TargetObject.GetObjectVariable<PersistentVariableFloat>("_ORIGINAL_SIZE").Value = onSpellCast.TargetObject.VisualTransform.Scale;

      onSpellCast.TargetObject.VisualTransform.Lerp(new VisualTransformLerpSettings { LerpType = VisualTransformLerpType.EaseIn, Duration = TimeSpan.FromSeconds(2), PauseWithGame = true }, transform => { transform.Scale = onSpellCast.TargetObject.GetObjectVariable<PersistentVariableFloat>("_ORIGINAL_SIZE").Value * 2; });
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, EffectSystem.strengthAdvantage, NwTimeSpan.FromRounds(spellEntry.duration));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, EffectSystem.enlargeEffect, NwTimeSpan.FromRounds(spellEntry.duration));

      if (caster.GetObjectVariable<LocalVariableInt>("_ENLARGE_DUERGAR").HasNothing)
        EffectSystem.ApplyConcentrationEffect(caster, onSpellCast.Spell.Id, new List<NwGameObject> { onSpellCast.TargetObject }, spellEntry.duration);
      else
        caster.GetObjectVariable<LocalVariableInt>("_ENLARGE_DUERGAR").Delete();
    }
  }
}
