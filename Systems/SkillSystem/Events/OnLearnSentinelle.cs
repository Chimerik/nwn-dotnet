﻿using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnSentinelle(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Sentinelle))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.Sentinelle);

      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackSentinelle;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackSentinelle;

      List<NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(new("Force", (int)Ability.Strength));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
        abilities.Add(new("Dextérité", (int)Ability.Dexterity));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      return true;
    }
  }
}
