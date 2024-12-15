using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDruideGardien(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.windows.TryGetValue("spellSelection", out var spell1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Druid, 2));
      else ((SpellSelectionWindow)spell1).CreateWindow(ClassType.Druid, 2);

      if (!player.oid.LoginCreature.KnowsFeat(Feat.ArmorProficiencyMedium))
        player.oid.LoginCreature.AddFeat(Feat.ArmorProficiencyMedium);

      player.learnableSkills.TryAdd(CustomSkill.MediumArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MediumArmorProficiency], player));
      player.learnableSkills[CustomSkill.MediumArmorProficiency].source.Add(Category.Class);

      if (player.learnableSkills[CustomSkill.MediumArmorProficiency].currentLevel < 1)
        player.learnableSkills[CustomSkill.MediumArmorProficiency].LevelUp(player);

      player.learnableSkills.TryAdd(CustomSkill.LightFlailProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightFlailProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.BattleaxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BattleaxeProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.MorningstarProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MorningstarProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.GreatswordProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.GreatswordProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.GreataxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.GreataxeProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.HalberdProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HalberdProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.ScimitarProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ScimitarProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.ThrowingAxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ThrowingAxeProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.HeavyFlailProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HeavyFlailProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.WarHammerProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarHammerProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.HeavyCrossbowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HeavyCrossbowProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.LongBowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LongBowProficiency], player));
      player.learnableSkills.TryAdd(CustomSkill.WhipProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WhipProficiency], player));

      return true;
    }
  }
}
