using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Rogue
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.LightArmorProficiency],
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.DexteritySavesProficiency],
        learnableDictionary[CustomSkill.IntelligenceSavesProficiency],
      },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.ShurikenProficiency],
        learnableDictionary[CustomSkill.LongSwordProficiency],
        learnableDictionary[CustomSkill.RapierProficiency],
        learnableDictionary[CustomSkill.ShortSwordProficiency],
      },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.AcrobaticsProficiency],
        learnableDictionary[CustomSkill.AthleticsProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.IntimidationProficiency],
        learnableDictionary[CustomSkill.PerceptionProficiency],
        learnableDictionary[CustomSkill.DeceptionProficiency],
        learnableDictionary[CustomSkill.InvestigationProficiency],
        learnableDictionary[CustomSkill.PerformanceProficiency],
        learnableDictionary[CustomSkill.PersuasionProficiency],
        learnableDictionary[CustomSkill.SleightOfHandProficiency],
        learnableDictionary[CustomSkill.StealthProficiency],
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];

      HandleRogueLevelUp(player, playerClass.currentLevel, playerClass);

      /*switch (customSkillId)
      {
        case CustomSkill.FighterChampion: HandleChampionLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.FighterArcaneArcher: HandleArcherMageLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.FighterWarMaster: HandleWarMasterLevelUp(player, playerClass.currentLevel); break;
      }*/

      if(playerClass.currentLevel > 1)
        player.oid.LoginCreature.ForceLevelUp(CustomClass.Rogue, player.RollClassHitDie(customSkillId, CustomClass.Rogue, player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));

      player.GiveRacialBonusOnLevelUp();

      return true;
    }
  }
}
