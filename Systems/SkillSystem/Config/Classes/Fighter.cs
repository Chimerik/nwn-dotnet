using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static readonly List<int> availableSkills = new() 
    { 
      CustomSkill.AcrobaticsProficiency, 
      CustomSkill.AnimalHandlingProficiency, 
      CustomSkill.AthleticsProficiency, 
      CustomSkill.HistoryProficiency,
      CustomSkill.InsightProficiency,
      CustomSkill.IntimidationProficiency,
      CustomSkill.PerceptionProficiency,
      CustomSkill.SurvivalProficiency
    };

    private static readonly StartingPackage startingPackage = new(
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
        learnableDictionary[CustomSkill.MartialWeaponProficiency]
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
        learnableDictionary[CustomSkill.SurvivalProficiency],
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];

      HandleFighterLevelUp(player, playerClass.currentLevel);

      switch (customSkillId)
      {
        case CustomSkill.FighterChampion: HandleChampionLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.FighterArcaneArcher: HandleArcherMageLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.FighterWarMaster: HandleWarMasterLevelUp(player, playerClass.currentLevel); break;


      }

      if(playerClass.currentLevel > 1)
      {
        int classPosition = GetFighterClassPosition(player);

        if (classPosition < 0)
          LogUtils.LogMessage($"LEVEL UP ERROR - {player.oid.LoginCreature.Name} ({player.oid.PlayerName}) - Impossible de trouver la classe Guerrier - Level up {playerClass.currentLevel}", LogUtils.LogType.PlayerSaveSystem);
        else
          CreaturePlugin.SetLevelByPosition(player.oid.LoginCreature, classPosition, playerClass.currentLevel);
      }

      player.RollClassHitDie(customSkillId, CustomClass.Fighter);
      player.GiveRacialBonusOnLevelUp();

      return true;
    }

    private static int GetFighterClassPosition(Player player)
    {
      for (int i = 0; i < player.oid.LoginCreature.Classes.Count; i++)
      {
        switch (player.oid.LoginCreature.Classes[i].Class.Id)
        {
          case CustomClass.Fighter:
          case CustomClass.Warmaster:
          case CustomClass.Champion:
          case CustomClass.EldritchKnight:
          case CustomClass.ArcaneArcher: return i;
        }
      }

      return -1;
    }
  }
}
