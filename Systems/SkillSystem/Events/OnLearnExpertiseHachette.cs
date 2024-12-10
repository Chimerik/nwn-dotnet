﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnExpertiseHachette(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.LearnClassSkill(CustomSkill.ExpertisePreparation);
      player.LearnClassSkill(CustomSkill.ExpertiseMutilation);
      player.LearnClassSkill(CustomSkill.ExpertiseEntaille);

      return true;
    }
  }
}
