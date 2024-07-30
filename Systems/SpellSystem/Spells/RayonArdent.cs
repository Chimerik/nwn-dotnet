using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RayonArdent(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, NwFeat feat = null)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (feat is not null && feat.Id == CustomSkill.MonkEtreinteDeLenfer)
      {
        caster.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(caster, 3);
        castingClass = NwClass.FromClassId(CustomClass.Monk);
      }

      List<NwGameObject> targets = new();

      if (caster.IsLoginPlayerCharacter)
      {
        int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;

        if (nbTargets < 1)
          return;

        for (int i = 0; i < nbTargets; i++)
        {
          targets.Add(caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Value);
          caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Delete();
        }

        caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
      }
      else
        foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 36, true).Take(3))
          targets.Add(target);
      
      SpellUtils.SignalEventSpellCast(caster, caster, Spell.Firebrand);
      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      foreach (var target in targets)
      {
        float distance = target.DistanceSquared(caster);
        int targetNBDice = nbDice;

        if (-1 < distance && distance < 1300)
        {
          switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, castingClass.SpellCastingAbility))
          {
            case TouchAttackResult.CriticalHit: targetNBDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
            case TouchAttackResult.Hit: break;
            default: continue;
          }

          target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamFire, caster, BodyNode.Hand), TimeSpan.FromSeconds(1.2));
          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, targetNBDice, oCaster, spell.GetSpellLevelForClass(castingClass));
        }
        else
          caster.LoginPlayer?.SendServerMessage($"{target.Name} n'est plus à portée", ColorConstants.Orange);
      }
    }
  }
}
