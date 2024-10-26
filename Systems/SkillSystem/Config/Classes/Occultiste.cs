using System.Collections.Generic;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Occultiste
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.LightArmorProficiency],
        learnableDictionary[CustomSkill.WisdomSavesProficiency],
        learnableDictionary[CustomSkill.CharismaSavesProficiency],
      },
      new List<Learnable>()
      {
      },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.ArcanaProficiency],
        learnableDictionary[CustomSkill.HistoryProficiency],
        learnableDictionary[CustomSkill.DeceptionProficiency],
        learnableDictionary[CustomSkill.IntimidationProficiency],
        learnableDictionary[CustomSkill.InvestigationProficiency],
        learnableDictionary[CustomSkill.NatureProficiency],
        learnableDictionary[CustomSkill.ReligionProficiency]
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandleOccultisteLevelUp(player, playerClass.currentLevel, playerClass);
      
      switch (customSkillId)
      {
        //case CustomSkill.OccultisteArchifee: HandleArchifeeLevelUp(player, playerClass.currentLevel); break;
      }

      player.ApplyClassLevelUp(playerClass, CustomClass.Druid);

      return true;
    }
  }
}
