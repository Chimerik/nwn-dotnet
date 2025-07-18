﻿using System;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void SlowTrap(NwGameObject trap, TrapEntry entry)
    {
      foreach (NwCreature creature in trap.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, entry.aoeSize, false))
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        int advantage = CreatureUtils.GetCreatureSavingThrowAdvantage(creature, Ability.Dexterity, null, SpellConfig.SpellEffectType.Trap, trap);

        if (advantage < 900)
        {
          creature.LoginPlayer?.SendServerMessage($"Jet de dextérité contre les pièges : {"ECHEC AUTOMATIQUE".ColorString(ColorConstants.Red)}".ColorString(ColorConstants.Orange));

          creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
          NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Temporary, Effect.Slow(), TimeSpan.FromSeconds(entry.duration)));
          return;
        }

        int totalSave = SpellUtils.GetSavingThrowRoll(creature, Ability.Dexterity, entry.baseDC, advantage, feedback);
        SavingThrowResult saveResult = (SavingThrowResult)(totalSave >= entry.baseDC).ToInt(); // TODO : Variabiliser le DD selon la compétence de celui qui a posé le piège
        // TODO : Variabiliser la durée selon la compétence du crafteur
        creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
        NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Temporary, Effect.Slow(), TimeSpan.FromSeconds(entry.duration)));

        TrapUtils.SendSavingThrowFeedbackMessage(creature, feedback.saveRoll, feedback.proficiencyBonus, advantage, entry.baseDC, totalSave, saveResult, Ability.Dexterity);
      }
    }
  }
}
