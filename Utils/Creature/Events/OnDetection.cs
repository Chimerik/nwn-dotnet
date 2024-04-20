using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDetection(OnDoSpotDetection onSpot)
    {
      //StringUtils.DisplayStringToAllPlayersNearTarget(onSpot.Creature, $"Spot Check {onSpot.Creature.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Orange, true, true);

      if (!onSpot.Target.IsPlayerControlled && (onSpot.Target.Master is null || !onSpot.Target.Master.IsPlayerControlled)
        || onSpot.Target.IsReactionTypeFriendly(onSpot.Creature) || !onSpot.Target.GetActionMode(ActionMode.Stealth)
        || onSpot.Target.DistanceSquared(onSpot.Creature) > 600)
        return;

      //StringUtils.DisplayStringToAllPlayersNearTarget(onSpot.Creature, $"Spot Routine {onSpot.Creature.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Orange, true, true);

      if (onSpot.Creature.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 25, true).Any(c => c == onSpot.Target))
      {
        double angle = AngleBetween(onSpot.Creature.Position, onSpot.Target.Position);
        angle = angle < 0 ? 360 + angle : angle;

        float visionMax = onSpot.Creature.Rotation + 67 > 360 ? (onSpot.Creature.Rotation + 67) - 360 : onSpot.Creature.Rotation + 67;
        float visionMin = onSpot.Creature.Rotation - 67 < 0 ? 360 + (onSpot.Creature.Rotation - 67) : onSpot.Creature.Rotation - 67;

        if (visionMax > visionMin)
        {
          if (angle > visionMin && angle < visionMax)
          {
            if (GetSkillDuelResult(onSpot.Creature, onSpot.Target, new List<Ability>() { Ability.Wisdom }, new List<Ability>() { Ability.Dexterity },
              new List<int>() { CustomSkill.PerceptionProficiency }, new List<int>() { CustomSkill.StealthProficiency }, SpellConfig.SpellEffectType.Stealth, true))
            {
              onSpot.Target.SetActionMode(ActionMode.Stealth, false);
              StringUtils.DisplayStringToAllPlayersNearTarget(onSpot.Target, $"Repéré par {onSpot.Creature.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Orange, true, true);
            }
          }
        }
        else if((angle > visionMin) || (angle < visionMax))
        {
          if (GetSkillDuelResult(onSpot.Creature, onSpot.Target, new List<Ability>() { Ability.Wisdom }, new List<Ability>() { Ability.Dexterity },
          new List<int>() { CustomSkill.PerceptionProficiency }, new List<int>() { CustomSkill.StealthProficiency }, SpellConfig.SpellEffectType.Stealth, true))
          {
            onSpot.Target.SetActionMode(ActionMode.Stealth, false);
            StringUtils.DisplayStringToAllPlayersNearTarget(onSpot.Target, $"Repéré par {onSpot.Creature.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Orange, true, true);
          }
        }
      }
    }
  }
}
