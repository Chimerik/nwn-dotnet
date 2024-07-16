﻿using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ImmobilisationDePersonne(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targetList = new();

      if (oCaster is not NwCreature caster)
        return targetList;

      List<NwGameObject> targets = new();
      int nbTargets = oCaster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;
      int DC = SpellUtils.GetCasterSpellDC(caster, spell, castingClass.SpellCastingAbility);

      if (nbTargets > 0)
      {
        for (int i = 0; i < nbTargets; i++)
        {
          targets.Add(oCaster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Value);
          oCaster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Delete();
        }

        oCaster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
      }
      else
      {
        targets.Add(oTarget);
      }

      foreach (var target in targets.Distinct())
      {
        if (target is NwCreature targetCreature 
          && (targetCreature.IsPlayableRace || Utils.In(targetCreature.Race.RacialType, RacialType.HumanoidReptilian, RacialType.HumanoidMonstrous, RacialType.HumanoidGoblinoid, RacialType.HumanoidOrc))
        && !CreatureUtils.GetSavingThrow(caster, targetCreature, spellEntry.savingThrowAbility, DC, spellEntry))
        {
          NWScript.AssignCommand(caster, () => targetCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetImmobilisationDePersonneEffect(castingClass.SpellCastingAbility), NwTimeSpan.FromRounds(spellEntry.duration)));
          targetList.Add(target);
        }
      }

      return targetList;
    }
  }
}
