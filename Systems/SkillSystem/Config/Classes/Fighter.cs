using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static class Fighter
  {
    private static readonly StartingPackage startingPackage = new(
      new List<Learnable>()
      { 
        SkillSystem.learnableDictionary[CustomSkill.LightArmorProficiency],
        SkillSystem.learnableDictionary[CustomSkill.MediumArmorProficiency],
        SkillSystem.learnableDictionary[CustomSkill.ShieldProficiency],
        SkillSystem.learnableDictionary[CustomSkill.SimpleWeaponProficiency],
        SkillSystem.learnableDictionary[CustomSkill.StrengthSavesProficiency],
        SkillSystem.learnableDictionary[CustomSkill.ConstitutionSavesProficiency],
      },
      new List<Learnable>()
      {
        SkillSystem.learnableDictionary[CustomSkill.HeavyArmorProficiency],
        SkillSystem.learnableDictionary[CustomSkill.MartialWeaponProficiency]
      },
      new List<Learnable>()
      {
        SkillSystem.learnableDictionary[CustomSkill.AcrobaticsProficiency],
        SkillSystem.learnableDictionary[CustomSkill.AnimalHandlingProficiency],
        SkillSystem.learnableDictionary[CustomSkill.AthleticsProficiency],
        SkillSystem.learnableDictionary[CustomSkill.HistoryProficiency],
        SkillSystem.learnableDictionary[CustomSkill.InsightProficiency],
        SkillSystem.learnableDictionary[CustomSkill.IntimidationProficiency],
        SkillSystem.learnableDictionary[CustomSkill.PerceptionProficiency],
        SkillSystem.learnableDictionary[CustomSkill.SurvivalProficiency],
      },
      2
    );

    public static bool LevelUp(PlayerSystem.Player player, int customSkillId)
    {
      switch(player.learnableSkills[customSkillId].currentLevel)
      {
        case 1:

          // Si c'est le tout premier niveau, on donne le starting package
          if (player.oid.LoginCreature.Classes[0]?.Class.ClassType == (ClassType)43)
          {
            foreach (Learnable learnable in startingPackage.freeLearnables)
              if (player.learnableSkills.TryAdd(learnable.id, new LearnableSkill((LearnableSkill)learnable, (int)SkillSystem.Category.Class)))
                player.learnableSkills[learnable.id].LevelUp(player);

            foreach (Learnable learnable in startingPackage.learnables)
              player.learnableSkills.TryAdd(learnable.id, new LearnableSkill((LearnableSkill)learnable, (int)SkillSystem.Category.Class));

            // TODO : Demander de choisir deux skills parmi la liste

            CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, 0, (int)ClassType.Fighter);
          }
          else
            player.oid.LoginCreature.LevelUp(NwClass.FromClassType(ClassType.Fighter), 1);

          // On donne les autres capacités de niveau 1
          if (player.learnableSkills.TryAdd(CustomSkill.FighterSecondWind, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.FighterSecondWind], (int)SkillSystem.Category.Class)))
            player.learnableSkills[CustomSkill.FighterSecondWind].LevelUp(player);

          // TODO : Donner le choix d'une style de combat

          break;

        case 2:

          player.oid.LoginCreature.LevelUp(NwClass.FromClassType(ClassType.Fighter), 1);

          if (player.learnableSkills.TryAdd(CustomSkill.FighterSurge, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.FighterSurge], (int)SkillSystem.Category.Class)))
            player.learnableSkills[CustomSkill.FighterSurge].LevelUp(player);  

          break;

        case 3:

          player.oid.LoginCreature.LevelUp(NwClass.FromClassType(ClassType.Fighter), 1);

          // TODO : Donner le choix d'un archétype martial

          break;
      }

      return true;
    }
  }
}
