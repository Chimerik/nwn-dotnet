using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnProtectionLegere(PlayerSystem.Player player, int customSkillId)
    {
      List<NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(new("Force", (int)Ability.Strength));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
        abilities.Add(new("Dextérité", (int)Ability.Dexterity));

      if (!player.oid.LoginCreature.KnowsFeat(Feat.ArmorProficiencyLight))
        player.oid.LoginCreature.AddFeat(Feat.ArmorProficiencyLight);

      player.learnableSkills.TryAdd(CustomSkill.LightArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightArmorProficiency], player));
      player.learnableSkills[CustomSkill.LightArmorProficiency].source.Add(Category.Feat);

      if (player.learnableSkills[CustomSkill.LightArmorProficiency].currentLevel < 1)
        player.learnableSkills[CustomSkill.LightArmorProficiency].LevelUp(player);

      if (!player.oid.LoginCreature.KnowsFeat(Feat.ShieldProficiency))
        player.oid.LoginCreature.AddFeat(Feat.ShieldProficiency);

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      return true;
    }
  }
}
