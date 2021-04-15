﻿using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  class InstantLearn
  {
    public InstantLearn(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionner la cible de l'apprentissage instantanné.");
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, SelectLearnTarget, ObjectTypes.Creature, MouseCursor.Create);
    }
    private void SelectLearnTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.TargetObject == null || !(selection.TargetObject is NwPlayer)
        || !PlayerSystem.Players.TryGetValue(selection.TargetObject, out PlayerSystem.Player targetPlayer))
        return;

      if (targetPlayer.currentSkillType == SkillType.Skill)
        targetPlayer.learnableSkills[(Feat)targetPlayer.currentSkillJob].acquiredPoints = targetPlayer.learnableSkills[(Feat)targetPlayer.currentSkillJob].pointsToNextLevel;
      else if (targetPlayer.currentSkillType == SkillType.Spell)
        targetPlayer.learnableSpells[targetPlayer.currentSkillJob].acquiredPoints = targetPlayer.learnableSpells[targetPlayer.currentSkillJob].pointsToNextLevel;
    }
  }
}
