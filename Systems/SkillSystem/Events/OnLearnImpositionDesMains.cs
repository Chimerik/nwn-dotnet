﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnImpositionDesMains(PlayerSystem.Player player, int customSkillId)
    {
      NwCreature creature = player.oid.LoginCreature;

      if (!creature.KnowsFeat((Feat)customSkillId))
        creature.AddFeat((Feat)customSkillId);

      creature.SetFeatRemainingUses((Feat)customSkillId, 3);

      return true;
    }
  }
}
