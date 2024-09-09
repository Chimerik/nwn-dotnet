using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Monk
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.ShortSwordProficiency],
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.DexteritySavesProficiency],
        learnableDictionary[CustomSkill.StrengthSavesProficiency],
      },
      new List<Learnable>() { },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.AcrobaticsProficiency],
        learnableDictionary[CustomSkill.AthleticsProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.StealthProficiency],
        learnableDictionary[CustomSkill.ReligionProficiency],
        learnableDictionary[CustomSkill.HistoryProficiency],
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandleMonkLevelUp(player, playerClass.currentLevel, playerClass);

      switch (customSkillId)
      {
        case CustomSkill.MonkPaume: HandlePaumeLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.MonkOmbre: HandleOmbreLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.MonkElements: HandleElementsLevelUp(player, playerClass.currentLevel); break;
      }

      player.ApplyClassLevelUp(playerClass, CustomClass.Monk);

      return true;
    }
  }
}
