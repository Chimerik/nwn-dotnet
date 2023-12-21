﻿using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnExpert(PlayerSystem.Player player, int customSkillId)
    {
      foreach (var learnable in player.learnableSkills.Values.Where(s => (s.category == Category.Skill 
      || s.category == Category.Expertise) && s.currentLevel < 1))
        learnable.acquiredPoints += learnable.pointsToNextLevel / 4;

      List<Ability> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(Ability.Strength);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
        abilities.Add(Ability.Dexterity);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) < 20)
        abilities.Add(Ability.Constitution);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence) < 20)
        abilities.Add(Ability.Intelligence);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Wisdom) < 20)
        abilities.Add(Ability.Wisdom);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma) < 20)
        abilities.Add(Ability.Charisma);

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      return true;
    }
  }
}
