using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Druide
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.LightArmorProficiency],
        learnableDictionary[CustomSkill.ShieldProficiency],
        learnableDictionary[CustomSkill.WisdomSavesProficiency],
        learnableDictionary[CustomSkill.IntelligenceSavesProficiency],
      },
      new List<Learnable>()
      {
      },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.ArcanaProficiency],
        learnableDictionary[CustomSkill.AnimalHandlingProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.MedicineProficiency],
        learnableDictionary[CustomSkill.NatureProficiency],
        learnableDictionary[CustomSkill.PerceptionProficiency],
        learnableDictionary[CustomSkill.ReligionProficiency],
        learnableDictionary[CustomSkill.SurvivalProficiency]
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandleDruideLevelUp(player, playerClass.currentLevel, playerClass);
      
      switch (customSkillId)
      {
        //case CustomSkill.EnsorceleurLigneeDraconique: HandleDraconiqueLevelUp(player, playerClass.currentLevel); break;
        //case CustomSkill.EnsorceleurTempete: HandleTempeteLevelUp(player, playerClass.currentLevel); break;
      }

      player.ApplyClassLevelUp(playerClass, CustomClass.Druid);

      return true;
    }
  }
}
