﻿using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRestaurationArcanique(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.WizardRestaurationArcanique, (byte)Math.Round((double)(player.oid.LoginCreature.GetClassInfo(ClassType.Wizard).Level / 2), MidpointRounding.AwayFromZero));
      return true;
    }
  }
}
