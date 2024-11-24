using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMeneurExaltant(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MeneurExaltant))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MeneurExaltant);

      List<NuiComboEntry> abilities = new();

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
