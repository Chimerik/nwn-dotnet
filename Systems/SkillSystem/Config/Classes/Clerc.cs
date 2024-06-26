﻿using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.LightArmorProficiency],
        learnableDictionary[CustomSkill.MediumArmorProficiency],
        learnableDictionary[CustomSkill.ShieldProficiency],
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.MorningstarProficiency],
        learnableDictionary[CustomSkill.CharismaSavesProficiency],
        learnableDictionary[CustomSkill.WisdomSavesProficiency],
      },
      new List<Learnable>()
      {
      },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.HistoryProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.MedicineProficiency],
        learnableDictionary[CustomSkill.PersuasionProficiency],
        learnableDictionary[CustomSkill.ReligionProficiency]
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandleClercLevelUp(player, playerClass.currentLevel, playerClass);
      
      switch (customSkillId)
      {
        case CustomSkill.ClercDuperie: HandleDuperieLevelUp(player, playerClass.currentLevel); break;
        //case CustomSkill.PaladinSermentDesAnciens: HandleAnciensLevelUp(player, playerClass.currentLevel); break;
        //case CustomSkill.PaladinSermentVengeance: HandleVengeanceLevelUp(player, playerClass.currentLevel); break;
      }

      if (player.oid.LoginCreature.Level > 2 || playerClass.currentLevel > 1)
      {
        player.oid.LoginCreature.ForceLevelUp(CustomClass.Clerc, player.RollClassHitDie(player.oid.LoginCreature.Level, CustomClass.Clerc, player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));
      }
      
      player.GiveRacialBonusOnLevelUp();

      return true;
    }
  }
}
