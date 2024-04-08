using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        learnableDictionary[CustomSkill.WisdomSavesProficiency],
        learnableDictionary[CustomSkill.IntelligenceSavesProficiency],
        learnableDictionary[CustomSkill.DaggerProficiency],
        learnableDictionary[CustomSkill.DartProficiency],
        learnableDictionary[CustomSkill.SlingProficiency],
        learnableDictionary[CustomSkill.QuarterstaffProficiency],
        learnableDictionary[CustomSkill.MagicStaffProficiency],
        learnableDictionary[CustomSkill.LightCrossbowProficiency]
      },
      new List<Learnable>()
      {
        
      },
      new List<Learnable>()
      {
        learnableDictionary[CustomSkill.ArcanaProficiency],
        learnableDictionary[CustomSkill.HistoryProficiency],
        learnableDictionary[CustomSkill.InsightProficiency],
        learnableDictionary[CustomSkill.InvestigationProficiency],
        learnableDictionary[CustomSkill.MedicineProficiency],
        learnableDictionary[CustomSkill.ReligionProficiency]
      },
      4
    );

    public static bool LevelUp(Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];
      HandleWizardLevelUp(player, playerClass.currentLevel, playerClass);

      switch (customSkillId)
      {
        case CustomSkill.WizardAbjuration: HandleAbjurationLevelUp(player, playerClass.currentLevel); break;
        case CustomSkill.WizardDivination: HandleDivinationLevelUp(player, playerClass.currentLevel); break;
      }

      if (playerClass.currentLevel > 2 || playerClass.currentLevel > 1)
        player.oid.LoginCreature.ForceLevelUp(CustomClass.Wizard, player.RollClassHitDie(player.oid.LoginCreature.Level, CustomClass.Wizard, player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));

      player.GiveRacialBonusOnLevelUp();

      return true;
    }
  }
}
