using Anvil.API.Events;
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
        int totalScore = perceptionRoll + perceptionSkill;
        DetectPlaceableTraps(onHB.Creature, totalScore);
        DetectDoorTraps(onHB.Creature, totalScore);
        DetectTriggerTraps(onHB.Creature, totalScore);
      }
      catch (Exception)
      {
        onHB.Creature.LoginPlayer?.DisplayFloatingTextStringOnCreature(onHB.Creature, "Erreur NwSound : Qu'étiez vous en train de faire à l'instant ?".ColorString(ColorConstants.Red));
      }
    }
    private static void DetectPlaceableTraps(NwCreature detector, int detectionScore)
    {
      foreach (var plc in detector.Location.GetObjectsInShapeByType<NwPlaceable>(Shape.Sphere,
        detector.DetectModeActive || detector.KnowsFeat(Feat.KeenSense) ? 6.66f : 3.33f, false))
      {
        if (!plc.IsTrapped || plc.IsTrapDetectedBy(detector))
          continue;

        if (plc.TrapDetectDC < detectionScore)
          plc.SetTrapDetectedBy(true, detector);
      }
    }
    private static void DetectDoorTraps(NwCreature detector, int detectionScore)
    {
      foreach (var door in detector.Location.GetObjectsInShapeByType<NwDoor>(Shape.Sphere,
        detector.DetectModeActive || detector.KnowsFeat(Feat.KeenSense) ? 6.66f : 3.33f, false))
      {
        if (!door.IsTrapped || door.IsTrapDetectedBy(detector))
          continue;

        if (door.TrapDetectDC < detectionScore)
          door.SetTrapDetectedBy(true, detector);
      }
    }
    private static void DetectTriggerTraps(NwCreature detector, int detectionScore)
    {
      foreach (var trigger in detector.Location.GetObjectsInShapeByType<NwTrigger>(Shape.Sphere,
        detector.DetectModeActive || detector.KnowsFeat(Feat.KeenSense) ? 6.66f : 3.33f, false))
      {
        if (!trigger.IsTrapped || trigger.IsTrapDetectedBy(detector))
          continue;

        if (trigger.TrapDetectDC < detectionScore)
          trigger.SetTrapDetectedBy(true, detector);
      }
    }
  }
}
