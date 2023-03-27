using Anvil.API;
using System;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(SpellSystem))]
  public partial class SpellSystem
  {
    public static void RayOfFrost(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);

      Effect eVis = Effect.VisualEffect(VfxType.ImpFrostS);
      Effect eRay = Effect.Beam(VfxType.BeamCold, oCaster, BodyNode.Hand);

      if (oCaster.CheckResistSpell(onSpellCast.TargetObject) == ResistSpellResult.Failed)
      {
        int nDamage = SpellUtils.MaximizeOrEmpower(4, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat);
        Effect eDam = Effect.Damage(nDamage, DamageType.Cold);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eVis);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eDam);
      }

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eRay, TimeSpan.FromSeconds(1.7));
    }
  }
}
