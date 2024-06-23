using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.LightArmorProficiency],
        learnableDictionary[CustomSkill.MediumArmorProficiency],
        learnableDictionary[CustomSkill.ShieldProficiency],
        learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        learnableDictionary[CustomSkill.WisdomSavesProficiency],
        learnableDictionary[CustomSkill.CharismaSavesProficiency],
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
        learnableDictionary[CustomSkill.IntimidationProficiency],
        learnableDictionary[CustomSkill.MedicineProficiency],
        learnableDictionary[CustomSkill.AthleticsProficiency],
        learnableDictionary[CustomSkill.PersuasionProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.ReligionProficiency]
      },
      2
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandlePaladinLevelUp(player, playerClass.currentLevel, playerClass);
      
      switch (customSkillId)
      {
        case CustomSkill.PaladinSermentDevotion: HandleDevotionLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.PaladinSermentDesAnciens: HandleAnciensLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.PaladinSermentVengeance: HandleVengeanceLevelUp(player, playerClass.currentLevel); break;
      }

      if (player.oid.LoginCreature.Level > 2 || playerClass.currentLevel > 1)
      {
        player.oid.LoginCreature.ForceLevelUp(CustomClass.Paladin, player.RollClassHitDie(player.oid.LoginCreature.Level, CustomClass.Paladin, player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));
      }
      
      player.GiveRacialBonusOnLevelUp();

      return true;
    }
  }
}
