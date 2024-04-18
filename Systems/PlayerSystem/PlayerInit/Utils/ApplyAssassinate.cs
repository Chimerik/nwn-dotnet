﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAssassinate()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AssassinAssassinate))
        {
          oid.OnCombatStatusChange -= RogueUtils.OnCombatThiefReflex;
          oid.OnCombatStatusChange += RogueUtils.OnCombatThiefReflex;
        }
      }
    }
  }
}
