using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnDetection(OnDoSpotDetection onSpot)
    {
      if(onSpot.Target.GetActionMode(ActionMode.Stealth) && onSpot.Target.DistanceSquared(onSpot.Creature) < 600
        && onSpot.Creature.Location.GetObjectsInShapeByType<NwCreature>(Shape.Cone, 135, true, onSpot.Target.Position).Any(c => c == onSpot.Target))
      {
        if(GetSkillDuelResult(onSpot.Creature, onSpot.Target, new List<Ability>() { Ability.Dexterity }, new List<Ability>() { Ability.Wisdom },
          new List<int>() { CustomSkill.StealthProficiency }, new List<int>() { CustomSkill.PerceptionProficiency }))
        {
          onSpot.Target.SetActionMode(ActionMode.Stealth, false);
          StringUtils.DisplayStringToAllPlayersNearTarget(onSpot.Target, $"Repéré par {onSpot.Creature.Name}", ColorConstants.Orange, true);
        }
      }
    }
  }
}
