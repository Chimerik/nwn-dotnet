﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnExpertiseMasseLegere(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.LearnClassSkill(CustomSkill.ExpertiseCommotion);
      player.LearnClassSkill(CustomSkill.ExpertiseArretCardiaque);
      player.LearnClassSkill(CustomSkill.ExpertiseEraflure);

      return true;
    }
  }
}
