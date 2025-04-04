﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnExpertiseArcLong(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.LearnClassSkill(CustomSkill.ExpertiseStabilisation);
      player.LearnClassSkill(CustomSkill.ExpertiseCoupeJarret);

      return true;
    }
  }
}
