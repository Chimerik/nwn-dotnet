﻿using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Ensorceleur
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.DaggerProficiency],
        learnableDictionary[CustomSkill.QuarterstaffProficiency],
        learnableDictionary[CustomSkill.LightCrossbowProficiency],
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.CharismaSavesProficiency],
        learnableDictionary[CustomSkill.ConstitutionSavesProficiency],
      },
      new List<Learnable>()
      {
      },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.ArcanaProficiency],
        learnableDictionary[CustomSkill.DeceptionProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.IntimidationProficiency],
        learnableDictionary[CustomSkill.PersuasionProficiency],
        learnableDictionary[CustomSkill.ReligionProficiency]
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandleEnsorceleurLevelUp(player, playerClass.currentLevel, playerClass);
      
      switch (customSkillId)
      {
        case CustomSkill.EnsorceleurLigneeDraconique: HandleDraconiqueLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.EnsorceleurTempete: HandleTempeteLevelUp(player, playerClass.currentLevel); break;
      }

      player.ApplyClassLevelUp(playerClass, CustomClass.Ensorceleur);

      return true;
    }
  }
}
