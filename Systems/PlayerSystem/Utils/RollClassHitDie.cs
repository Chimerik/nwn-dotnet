﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void RollClassHitDie(int skillId, byte classId)
      {
        byte hitDie = NwClass.FromClassId(classId).HitDie;
        oid.LoginCreature.LevelInfo[oid.LoginCreature.LevelInfo.Count - 1].HitDie
          = learnableSkills[skillId].currentLevel < 2 ? hitDie : (byte)Utils.random.Next(hitDie / 2 + 1, hitDie + 1);
      }
    }
  }
}
