using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.LightArmorProficiency],
        learnableDictionary[CustomSkill.MediumArmorProficiency],
        learnableDictionary[CustomSkill.ShieldProficiency],
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.StrengthSavesProficiency],
        learnableDictionary[CustomSkill.ConstitutionSavesProficiency],
      },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.HeavyArmorProficiency],
        learnableDictionary[CustomSkill.LightFlailProficiency],
        learnableDictionary[CustomSkill.MorningstarProficiency],
        learnableDictionary[CustomSkill.BattleaxeProficiency],
        learnableDictionary[CustomSkill.GreataxeProficiency],
        learnableDictionary[CustomSkill.GreatswordProficiency],
        learnableDictionary[CustomSkill.ScimitarProficiency],
        learnableDictionary[CustomSkill.HalberdProficiency],
        learnableDictionary[CustomSkill.HeavyFlailProficiency],
        learnableDictionary[CustomSkill.ThrowingAxeProficiency],
        learnableDictionary[CustomSkill.TridentProficiency],
        learnableDictionary[CustomSkill.WarHammerProficiency],
        learnableDictionary[CustomSkill.HeavyCrossbowProficiency],
        learnableDictionary[CustomSkill.RapierProficiency],
        learnableDictionary[CustomSkill.ShortSwordProficiency],
        learnableDictionary[CustomSkill.LongSwordProficiency],
        learnableDictionary[CustomSkill.LongBowProficiency],
        learnableDictionary[CustomSkill.ShurikenProficiency],
        learnableDictionary[CustomSkill.WhipProficiency],
      },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.AcrobaticsProficiency],
        learnableDictionary[CustomSkill.AnimalHandlingProficiency],
        learnableDictionary[CustomSkill.AthleticsProficiency],
        learnableDictionary[CustomSkill.HistoryProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.IntimidationProficiency],
        learnableDictionary[CustomSkill.PerceptionProficiency],
        learnableDictionary[CustomSkill.PersuasionProficiency],
        learnableDictionary[CustomSkill.SurvivalProficiency],
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandleFighterLevelUp(player, playerClass.currentLevel, playerClass);

      switch (customSkillId)
      {
        case CustomSkill.FighterChampion: HandleChampionLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.FighterArcaneArcher: HandleArcherMageLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.FighterWarMaster: HandleWarMasterLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.FighterEldritchKnight: HandleEldritchKnightLevelUp(player, playerClass.currentLevel); break;
      }

      player.ApplyClassLevelUp(playerClass, customSkillId == CustomSkill.FighterEldritchKnight ? CustomClass.EldritchKnight : CustomClass.Fighter);

      return true;
    }
  }
}
