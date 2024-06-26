﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static int HandleInflexible(NwCreature creature, int saveRoll)
    {
      int? fighterLevel = FighterUtils.GetFighterLevel(creature);

      if (!fighterLevel.HasValue || fighterLevel.Value < 9)
        return saveRoll;

      int malus = fighterLevel.Value < 13 ? 2 : fighterLevel.Value < 17 ? 1 : 0;
      int reroll = NwRandom.Roll(Utils.random, 20) - malus;

      StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Inflexible", StringUtils.gold, true);
      LogUtils.LogMessage($"Echec JDS => Inflexible rerolled => {reroll}", LogUtils.LogType.Combat);

      return reroll;
    }
  }
}
