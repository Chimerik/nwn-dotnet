using System;
using System.Collections.Generic;
using NWN.Enums;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static Dictionary<int, Func<PlayerSystem.Player, int, int>> RegisterAddCustomFeatEffect = new Dictionary<int, Func<PlayerSystem.Player, int, int>>
    {
            { 1130, HandleAddStrengthMalusFeat },
    };

    public static Dictionary<int, Func<PlayerSystem.Player, int, int>> RegisterRemoveCustomFeatEffect = new Dictionary<int, Func<PlayerSystem.Player, int, int>>
    {
            { 1130, HandleRemoveStrengthMalusFeat },
    };

    private static int HandleAddStrengthMalusFeat(PlayerSystem.Player player, int idMalusFeat)
    {
      player.removeableMalus.Add(idMalusFeat, new Skill(idMalusFeat, 0));
      NWNX.Creature.SetRawAbilityScore(player, Enums.Ability.Strength, NWNX.Creature.GetRawAbilityScore(player, Enums.Ability.Strength) - 2);
      return 1;
    }

    private static int HandleRemoveStrengthMalusFeat(PlayerSystem.Player player, int idMalusFeat)
    {
      player.removeableMalus.Remove(idMalusFeat);
      NWNX.Creature.SetRawAbilityScore(player, Enums.Ability.Strength, NWNX.Creature.GetRawAbilityScore(player, Enums.Ability.Strength) + 2);

      return 1;
    }
  }
}
