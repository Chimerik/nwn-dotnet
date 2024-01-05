using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnHeartbeatDetectTrap(CreatureEvents.OnHeartbeat onHB)
    {
      if (onHB.Creature.Location is null || !Players.TryGetValue(onHB.Creature, out Player player))
        return;

      int perceptionRoll;
      int perceptionSkill = CreatureUtils.GetSkillScore(onHB.Creature, Ability.Wisdom, CustomSkill.PerceptionProficiency);

      if (player.learnableSkills.TryGetValue(CustomSkill.Observateur, out LearnableSkill observateur) && observateur.currentLevel > 0)
        perceptionSkill += 5;

      if (!onHB.Creature.DetectModeActive)
        perceptionSkill /= 2;

      foreach (var trap in onHB.Creature.Location.GetObjectsInShapeByType<NwGameObject>(Shape.Sphere, 
        onHB.Creature.DetectModeActive || onHB.Creature.KnowsFeat(Feat.KeenSense) ? 6.66f : 3.33f, true))
      {
        if(trap is NwPlaceable plc)
        {
          if (!plc.IsTrapped || plc.IsTrapDetectedBy(onHB.Creature))
            continue;

          perceptionRoll = NwRandom.Roll(Utils.random, 20);

          if (plc.TrapDetectDC < perceptionRoll +perceptionSkill)
            plc.SetTrapDetectedBy(true, onHB.Creature);
        }
        else if (trap is NwDoor door)
        {
          if (!door.IsTrapped || door.IsTrapDetectedBy(onHB.Creature))
            continue;

          perceptionRoll = NwRandom.Roll(Utils.random, 20);

          if (door.TrapDetectDC < perceptionRoll + perceptionSkill)
            door.SetTrapDetectedBy(true, onHB.Creature);
        }
        else if(trap is NwTrigger trigger)
        {
          if(!trigger.IsTrapped || trigger.IsTrapDetectedBy(onHB.Creature))
            continue;

          perceptionRoll = NwRandom.Roll(Utils.random, 20);

          if (trigger.TrapDetectDC < perceptionRoll + perceptionSkill)
            trigger.SetTrapDetectedBy(true, onHB.Creature);
        }
      }
    }
  }
}
