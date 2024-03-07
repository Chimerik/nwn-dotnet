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
      if (!onSpot.Target.IsPlayerControlled && (onSpot.Target.Master is null || !onSpot.Target.Master.IsPlayerControlled))
        return;

      if (onSpot.Target.GetActionMode(ActionMode.Stealth) && onSpot.Target.DistanceSquared(onSpot.Creature) < 600
        && onSpot.Creature.Location.GetObjectsInShapeByType<NwCreature>(Shape.Cone, 135, true, onSpot.Creature.Position).Any(c => c == onSpot.Target))
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
