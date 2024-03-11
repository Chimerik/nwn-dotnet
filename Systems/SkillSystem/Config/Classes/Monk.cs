using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Monk
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.ShortSwordProficiency],
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.DexteritySavesProficiency],
        learnableDictionary[CustomSkill.StrengthSavesProficiency],
      },
      new List<Learnable>() { },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.AcrobaticsProficiency],
        learnableDictionary[CustomSkill.AthleticsProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.StealthProficiency],
        learnableDictionary[CustomSkill.ReligionProficiency],
        learnableDictionary[CustomSkill.HistoryProficiency],
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];

      HandleMonkLevelUp(player, playerClass.currentLevel, playerClass);

      /*switch (customSkillId)
      {
        case CustomSkill.RogueThief: HandleThiefLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.RogueConspirateur: HandleConspirateurLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.RogueAssassin: HandleAssassinLevelUp(player, playerClass.currentLevel); break;
      }*/

      if (playerClass.currentLevel > 1)
        player.oid.LoginCreature.ForceLevelUp(CustomClass.Monk, player.RollClassHitDie(customSkillId, CustomClass.Rogue, player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));

      player.GiveRacialBonusOnLevelUp();

      return true;
    }
  }
}
