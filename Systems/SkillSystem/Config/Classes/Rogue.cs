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
      4
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandleRogueLevelUp(player, playerClass.currentLevel, playerClass);

      switch (customSkillId)
      {
        case CustomSkill.RogueThief: HandleThiefLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.RogueConspirateur: HandleConspirateurLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.RogueAssassin: HandleAssassinLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.RogueArcaneTrickster: HandleArcaneTricksterLevelUp(player, playerClass.currentLevel); break;
      }

      if (playerClass.currentLevel > 2 || playerClass.currentLevel > 1)
      {
        byte customClass = customSkillId == CustomSkill.RogueArcaneTrickster ? CustomClass.RogueArcaneTrickster : CustomClass.Rogue;
        player.oid.LoginCreature.ForceLevelUp(customClass, player.RollClassHitDie(player.oid.LoginCreature.Level, customClass, player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));
      }
      player.GiveRacialBonusOnLevelUp();

      return true;
    }
  }
}
