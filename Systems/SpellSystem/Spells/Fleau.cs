﻿using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Fleau(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      List<NwGameObject> objectTargets = new();

      if (oCaster is not NwCreature caster)
        return objectTargets;

      List<NwGameObject> targets = new();

      if (caster.IsLoginPlayerCharacter)
      {
        int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;

        if (nbTargets < 1)
          return objectTargets;

        for (int i = 0; i < nbTargets; i++)
        {
          targets.Add(caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Value);
          caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Delete();
        }

        caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
      }
      else
      {
        foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, true))
          targets.Add(target);
      }
      
      SpellUtils.SignalEventSpellCast(caster, caster, Spell.Bane);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosEvil10));

      foreach (var targetObject in targets.Distinct())
      {
        if (targetObject is NwCreature target)
        {
          float distance = target.DistanceSquared(caster);

          if (-1 < distance && distance < 90)
          {
            int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster);
            int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
            bool saveFailed = totalSave < spellDC;

            SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

            if (saveFailed)
            {
              objectTargets.Add(targetObject);
              target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadEvil));
              NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Fleau, NwTimeSpan.FromRounds(spellEntry.duration)));
            }
          }
          else
            caster.LoginPlayer?.SendServerMessage($"{target.Name} n'est plus à portée", ColorConstants.Orange);
        }
      }

      return objectTargets;
    }
  }
}
