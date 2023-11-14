using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnSportif(PlayerSystem.Player player, int customSkillId)
    {
      List<Ability> abilities = new List<Ability>() { Ability.Strength, Ability.Dexterity };

      if (!player.windows.ContainsKey("abilityBonusChoice")) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
      else ((AbilityBonusChoiceWindow)player.windows["abilityBonusChoice"]).CreateWindow(abilities);

      return true;
    }
  }
}
