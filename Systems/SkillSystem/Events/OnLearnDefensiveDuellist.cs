﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDefensiveDuellist(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(Feat.PrestigeDefensiveAwareness1);
      return true;
    }
  }
}
