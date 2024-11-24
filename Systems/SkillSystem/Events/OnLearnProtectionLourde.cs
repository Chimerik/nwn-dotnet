using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnProtectionLourde(PlayerSystem.Player player, int customSkillId)
    {
      List<NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(new("Force", (int)Ability.Strength));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) < 20)
        abilities.Add(new("Constitution", (int)Ability.Constitution));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var stat)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)stat).CreateWindow(abilities);
      }

      player.learnableSkills.TryAdd(CustomSkill.HeavyArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HeavyArmorProficiency], player));
      player.learnableSkills[CustomSkill.HeavyArmorProficiency].source.Add(Category.Feat);

      if (player.learnableSkills[CustomSkill.HeavyArmorProficiency].currentLevel < 1)
        player.learnableSkills[CustomSkill.HeavyArmorProficiency].LevelUp(player);
      
      return true;
    }
  }
}
