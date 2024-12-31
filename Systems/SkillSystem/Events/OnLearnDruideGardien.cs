using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDruideGardien(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.windows.TryGetValue("cantripSelection", out var spell1)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Druid, 2));
      else ((CantripSelectionWindow)spell1).CreateWindow(ClassType.Druid, 2);

      if (!player.oid.LoginCreature.KnowsFeat(Feat.ArmorProficiencyMedium))
        player.oid.LoginCreature.AddFeat(Feat.ArmorProficiencyMedium);

      player.learnableSkills.TryAdd(CustomSkill.MediumArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MediumArmorProficiency], player));
      player.learnableSkills[CustomSkill.MediumArmorProficiency].source.Add(Category.Class);

      if (player.learnableSkills[CustomSkill.MediumArmorProficiency].currentLevel < 1)
        player.learnableSkills[CustomSkill.MediumArmorProficiency].LevelUp(player);

      player.learnableSkills.TryAdd(CustomSkill.LightFlailProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightFlailProficiency], player));
      player.learnableSkills[CustomSkill.LightFlailProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.BattleaxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BattleaxeProficiency], player));
      player.learnableSkills[CustomSkill.BattleaxeProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.MorningstarProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MorningstarProficiency], player));
      player.learnableSkills[CustomSkill.MorningstarProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.GreatswordProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.GreatswordProficiency], player));
      player.learnableSkills[CustomSkill.GreatswordProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.GreataxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.GreataxeProficiency], player));
      player.learnableSkills[CustomSkill.GreataxeProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.HalberdProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HalberdProficiency], player));
      player.learnableSkills[CustomSkill.HalberdProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.ScimitarProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ScimitarProficiency], player));
      player.learnableSkills[CustomSkill.ScimitarProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.ThrowingAxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ThrowingAxeProficiency], player));
      player.learnableSkills[CustomSkill.ThrowingAxeProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.HeavyFlailProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HeavyFlailProficiency], player));
      player.learnableSkills[CustomSkill.HeavyFlailProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.WarHammerProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarHammerProficiency], player));
      player.learnableSkills[CustomSkill.WarHammerProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.HeavyCrossbowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HeavyCrossbowProficiency], player));
      player.learnableSkills[CustomSkill.HeavyCrossbowProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.LongBowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LongBowProficiency], player));
      player.learnableSkills[CustomSkill.LongBowProficiency].source.Add(Category.Class);
      player.learnableSkills.TryAdd(CustomSkill.WhipProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WhipProficiency], player));
      player.learnableSkills[CustomSkill.WhipProficiency].source.Add(Category.Class);

      return true;
    }
  }
}
