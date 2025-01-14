﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnPacteDeLaChaine(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.LearnAlwaysPreparedSpell(CustomSpell.AppelDeFamilier, CustomClass.Occultiste);

      return true;
    }
  }
}