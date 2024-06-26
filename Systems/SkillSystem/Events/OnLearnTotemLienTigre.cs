﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemLienTigre(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomSkill.TotemLienTigre;
      player.InitializeBonusSkillChoice();

      return true;
    }
  }
}
