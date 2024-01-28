using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Barbarian
  {
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
        learnableDictionary[CustomSkill.AnimalHandlingProficiency],
        learnableDictionary[CustomSkill.AthleticsProficiency],
        learnableDictionary[CustomSkill.NatureExpertise],
        learnableDictionary[CustomSkill.IntimidationProficiency],
        learnableDictionary[CustomSkill.PerceptionProficiency],
        learnableDictionary[CustomSkill.SurvivalProficiency]
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];

      HandleBarbarianLevelUp(player, playerClass.currentLevel, playerClass);

      /*switch (customSkillId)
      {
        case CustomSkill.FighterChampion: HandleBersekerLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.FighterArcaneArcher: HandleTotemLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.FighterWarMaster: HandleMagieSauvageLevelUp(player, playerClass.currentLevel); break;
      }*/

      if(playerClass.currentLevel > 1)
        player.oid.LoginCreature.ForceLevelUp(CustomClass.Barbarian, player.RollClassHitDie(customSkillId, CustomClass.Barbarian, player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));

      player.GiveRacialBonusOnLevelUp();

      return true;
    }
  }
}
