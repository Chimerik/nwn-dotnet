using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Bard
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.LightArmorProficiency],
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.ShurikenProficiency],
        learnableDictionary[CustomSkill.RapierProficiency],
        learnableDictionary[CustomSkill.ShortSwordProficiency],
        learnableDictionary[CustomSkill.LongSwordProficiency],
        learnableDictionary[CustomSkill.DexteritySavesProficiency],
        learnableDictionary[CustomSkill.CharismaSavesProficiency],
      },
      new List<Learnable>() { },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.AcrobaticsProficiency],
        learnableDictionary[CustomSkill.AnimalHandlingProficiency],
        learnableDictionary[CustomSkill.ArcanaProficiency],
        learnableDictionary[CustomSkill.AthleticsProficiency],
        learnableDictionary[CustomSkill.DeceptionProficiency],
        learnableDictionary[CustomSkill.HistoryProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.IntimidationProficiency],
        learnableDictionary[CustomSkill.InvestigationProficiency],
        learnableDictionary[CustomSkill.MedicineProficiency],
        learnableDictionary[CustomSkill.PerceptionProficiency],
        learnableDictionary[CustomSkill.PerformanceProficiency],
        learnableDictionary[CustomSkill.PersuasionProficiency],
        learnableDictionary[CustomSkill.ReligionProficiency],
        learnableDictionary[CustomSkill.SleightOfHandProficiency],
        learnableDictionary[CustomSkill.StealthProficiency],
        learnableDictionary[CustomSkill.SurvivalProficiency]
      },
      3
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandleBardLevelUp(player, playerClass.currentLevel, playerClass);

      switch (customSkillId)
      {
        case CustomSkill.BardCollegeDuSavoir: HandleCollegeDuSavoirLevelUp(player, playerClass.currentLevel); break;
        /*case CustomSkill.MonkOmbre: HandleOmbreLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.MonkElements: HandleElementsLevelUp(player, playerClass.currentLevel); break;*/
      }

      if (playerClass.currentLevel > 2 || playerClass.currentLevel > 1)
        player.oid.LoginCreature.ForceLevelUp(CustomClass.Bard, player.RollClassHitDie(player.oid.LoginCreature.Level, CustomClass.Bard, player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));

      player.GiveRacialBonusOnLevelUp();

      return true;
    }
  }
}
