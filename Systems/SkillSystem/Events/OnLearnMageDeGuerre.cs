﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMageDeGuerre(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MageDeGuerre))
        player.oid.LoginCreature.AddFeat(Feat.Ambidexterity);

      return true;
    }
  }
}
