﻿  using Anvil.API.Events;
using Anvil.API;
using System;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnHeartbeatDetectTrap(CreatureEvents.OnHeartbeat onHB)
    {
      if (onHB.Creature.Location is null || !Players.TryGetValue(onHB.Creature, out Player player))
        return;

      int perceptionRoll;
      int perceptionSkill = CreatureUtils.GetSkillScore(onHB.Creature, Ability.Wisdom, CustomSkill.PerceptionProficiency, true);

      if (player.learnableSkills.TryGetValue(CustomSkill.Observateur, out LearnableSkill observateur) && observateur.currentLevel > 0)
        perceptionSkill += 5;

      if (!onHB.Creature.DetectModeActive)
        perceptionSkill /= 2;

      perceptionRoll = RogueUtils.HandleSavoirFaire(onHB.Creature, CustomSkill.PerceptionProficiency,
        Utils.RollAdvantage(CreatureUtils.GetCreatureSkillAdvantage(onHB.Creature, CustomSkill.PerceptionProficiency), false));

      try
      {
        foreach (var trap in onHB.Creature.Location.GetObjectsInShapeByType<NwGameObject>(Shape.Sphere,
          onHB.Creature.DetectModeActive || onHB.Creature.KnowsFeat(Feat.KeenSense) ? 6.66f : 3.33f, false))
        {
          if (trap is NwPlaceable plc)
          {
            if (!plc.IsTrapped || plc.IsTrapDetectedBy(onHB.Creature))
              continue;

            if (plc.TrapDetectDC < perceptionRoll + perceptionSkill)
              plc.SetTrapDetectedBy(true, onHB.Creature);
          }
          else if (trap is NwDoor door)
          {
            if (!door.IsTrapped || door.IsTrapDetectedBy(onHB.Creature))
              continue;

            if (door.TrapDetectDC < perceptionRoll + perceptionSkill)
              door.SetTrapDetectedBy(true, onHB.Creature);
          }
          else if (trap is NwTrigger trigger)
          {
            if (!trigger.IsTrapped || trigger.IsTrapDetectedBy(onHB.Creature))
              continue;

            if (trigger.TrapDetectDC < perceptionRoll + perceptionSkill)
              trigger.SetTrapDetectedBy(true, onHB.Creature);
          }
        }
      }
      catch(Exception)
      {
        onHB.Creature.LoginPlayer?.DisplayFloatingTextStringOnCreature(onHB.Creature, "Erreur NwSound : Qu'étiez vous en train de faire à l'instant ?".ColorString(ColorConstants.Red));
      }
    }
  }
}
