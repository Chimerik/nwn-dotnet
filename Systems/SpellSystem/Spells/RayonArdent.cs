using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RayonArdent(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      List<NwGameObject> targets = new();

      if (caster.IsLoginPlayerCharacter)
      {
        targets.AddRange(SpellUtils.GetSpellTargets(caster, caster, spellEntry));
      }
      else
        foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 36, true).Take(3))
          if(caster.IsReactionTypeHostile(target))
            targets.Add(target);
      
      SpellUtils.SignalEventSpellCast(caster, caster, Spell.Firebrand);
      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      foreach (var target in targets)
      {
        int targetNBDice = nbDice;

        switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, castingClass.SpellCastingAbility))
        {
          case TouchAttackResult.CriticalHit: targetNBDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
          case TouchAttackResult.Hit: break;
          default:
            target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamFire, caster, BodyNode.Hand, true), TimeSpan.FromSeconds(1.2));
            continue;
        }

        target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamFire, caster, BodyNode.Hand), TimeSpan.FromSeconds(1.2));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, targetNBDice, oCaster, spell.GetSpellLevelForClass(castingClass));
      }
    }
  }
}
