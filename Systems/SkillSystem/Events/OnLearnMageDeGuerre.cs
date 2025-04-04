﻿using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMageDeGuerre(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MageDeGuerre))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MageDeGuerre);

      List<NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence) < 20)
        abilities.Add(new("Intelligence", (int)Ability.Intelligence));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Wisdom) < 20)
        abilities.Add(new("Sagesse", (int)Ability.Wisdom));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma) < 20)
        abilities.Add(new("Charisme", (int)Ability.Charisma));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      return true;
    }
  }
}
