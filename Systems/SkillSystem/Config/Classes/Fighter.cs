using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static class Fighter
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

    public static bool LevelUp(PlayerSystem.Player player, int customSkillId)
    {
      LearnableSkill playerClass = player.learnableSkills[customSkillId];

      switch(playerClass.currentLevel)
      {
        case 1:

          // Si c'est le tout premier niveau, on donne le starting package
          if (player.oid.LoginCreature.Level < 2)
          {
            foreach (Learnable learnable in startingPackage.freeLearnables)
            {
              if (player.learnableSkills.TryAdd(learnable.id, new LearnableSkill((LearnableSkill)learnable)))
                player.learnableSkills[learnable.id].LevelUp(player);

              player.learnableSkills[learnable.id].source.Add(Category.Class);
            }

            foreach (Learnable learnable in startingPackage.learnables)
            {
              player.learnableSkills.TryAdd(learnable.id, new LearnableSkill((LearnableSkill)learnable));
              player.learnableSkills[learnable.id].source.Add(Category.Class);
            }

            CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, 0, (int)ClassType.Fighter);
          }
          else
            CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, player.oid.LoginCreature.Classes.Count, (int)ClassType.Fighter);

          // On donne les autres capacités de niveau 1
           if (player.learnableSkills.TryAdd(CustomSkill.FighterSecondWind, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterSecondWind])))
            player.learnableSkills[CustomSkill.FighterSecondWind].LevelUp(player);

          player.learnableSkills[CustomSkill.FighterSecondWind].source.Add(Category.Class);

          int chosenStyle = player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CHOSEN_FIGHTER_STYLE").Value;

          if (player.learnableSkills.TryAdd(chosenStyle, new LearnableSkill((LearnableSkill)learnableDictionary[chosenStyle])))
            player.learnableSkills[chosenStyle].LevelUp(player);

          player.learnableSkills[chosenStyle].source.Add(Category.Class);

          break;

        case 2:

          if (player.learnableSkills.TryAdd(CustomSkill.FighterSurge, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterSurge])))
            player.learnableSkills[CustomSkill.FighterSurge].LevelUp(player);

          player.learnableSkills[CustomSkill.FighterSurge].source.Add(Category.Class);

          break;

        case 3:

          // TODO : Donner le choix d'un archétype martial

          break;
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

    private static int GetFighterClassPosition(PlayerSystem.Player player)
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
