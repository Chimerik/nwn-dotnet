﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDefensiveDuellist(PlayerSystem.Player player, int customSkillId)
    {
      byte rawDex = player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity);
      if (rawDex < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Dexterity, (byte)(rawDex + 1));

      player.oid.LoginCreature.AddFeat(Feat.PrestigeDefensiveAwareness1);
      return true;
    }
  }
}
