﻿using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void AcidTrap(List<NwCreature> targets, NwGameObject trap, TrapEntry entry)
    {
      foreach (var creature in targets)
      {
        Dictionary<string, bool> disadvantageDictionary = new()
        {
          { EffectSystem.ShieldArmorDisadvantageEffectTag, false } ,
        };

        Dictionary<string, bool> advantageDictionary = new()
        {
          { EffectSystem.DodgeEffectTag, false },
        };

        foreach (var eff in creature.ActiveEffects)
        {
          if (EffectUtils.IsIncapacitatingEffect(eff))
          {
            creature.LoginPlayer?.SendServerMessage($"Jet de dextérité contre les pièges : {"ECHEC AUTOMATIQUE".ColorString(ColorConstants.Red)}".ColorString(ColorConstants.Orange));

            creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
            NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, entry.damageDice, entry.numDice), entry.damageType)));
            NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Temporary, Effect.Paralyze(), TimeSpan.FromSeconds(entry.duration)));
            return;
          }

          advantageDictionary[EffectSystem.DodgeEffectTag] = advantageDictionary[EffectSystem.DodgeEffectTag] || EffectSystem.DodgeEffectTag == eff.Tag;
          disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] = disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] || EffectSystem.ShieldArmorDisadvantageEffectTag == eff.Tag;
        }

        SpellConfig.SavingThrowFeedback feedback = new();
        int advantage = -disadvantageDictionary.Count(v => v.Value) + advantageDictionary.Count(v => v.Value) + creature.KnowsFeat(Feat.KeenSense).ToInt() + (creature.Race.Id == CustomRace.Duergar).ToInt();
        int totalSave = SpellUtils.GetSavingThrowRoll(creature, Ability.Dexterity, entry.baseDC, advantage, feedback);
        int damage = NwRandom.Roll(Utils.random, entry.damageDice, entry.numDice); // TODO : Variabiliser les dégâts selon la compétence de l'artisan
        bool saveFailed = totalSave < entry.baseDC; // TODO : Variabiliser le DD selon la compétence de celui qui a posé le piège

        damage = ItemUtils.GetShieldMasterReducedDamage(creature, damage, saveFailed);

        if (saveFailed)
          NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Temporary, Effect.Paralyze(), TimeSpan.FromSeconds(entry.duration)));
        else
          damage /= 2;

        if (creature.KnowsFeat(Feat.KeenSense))
        {
          damage /= 2;
          creature?.LoginPlayer.DisplayFloatingTextStringOnCreature(creature, "Expert en donjons".ColorString(StringUtils.gold));
        }

        if (creature.IsLoginPlayerCharacter)
          TrapUtils.SendSavingThrowFeedbackMessage(creature, feedback.saveRoll, feedback.proficiencyBonus, advantage, entry.baseDC, totalSave, saveFailed, Ability.Dexterity);

        creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
        NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, entry.damageType)));
      }
    }
  }
}
