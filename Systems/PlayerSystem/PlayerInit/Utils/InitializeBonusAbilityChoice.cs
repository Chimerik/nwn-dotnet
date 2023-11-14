﻿using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeBonusAbilityChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ABILITY_BONUS_CHOICE_FEAT").HasValue)
        {
          List<Ability> abilities = new();

          for (int i = 0; i < 6; i++)
            if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>($"_IN_ABILITY_BONUS_CHOICE_FEAT_{i}").HasValue)
              abilities.Add((Ability)i);

          if (!windows.ContainsKey("abilityBonusChoice")) windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(this, abilities));
          else ((AbilityBonusChoiceWindow)windows["abilityBonusChoice"]).CreateWindow(abilities);
        }
      }
    }
  }
}
