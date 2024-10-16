﻿using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AssistanceTerrestre(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);
      DruideUtils.DecrementFormeSauvage(caster);

      foreach (var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        int nbDices = spellEntry.numDice + (caster.GetClassInfo(ClassType.Druid).Level > 9).ToInt() + (caster.GetClassInfo(ClassType.Druid).Level > 13).ToInt();

        if (caster.IsReactionTypeHostile(target))
        { 
          if(CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
          {
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcidS));
            SpellUtils.DealSpellDamage(target, caster.GetClassInfo(ClassType.Druid).Level, spellEntry, nbDices, caster, 2, SavingThrowResult.Failure);
          }            
        }
        else
        {
          int spellEffect = NwRandom.Roll(Utils.random, spellEntry.damageDice, nbDices);
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Heal(spellEffect)));
        }
      }
    }
  }
}
