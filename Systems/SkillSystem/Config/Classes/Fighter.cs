using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static class Fighter
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
      switch(player.learnableSkills[customSkillId].currentLevel)
      {
        case 1:

          // Si c'est le tout premier niveau, on donne le starting package
          if (player.oid.LoginCreature.Classes[0]?.Class.ClassType == (ClassType)43)
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

            // TODO : Demander de choisir deux skills parmi la liste

            CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, 0, (int)ClassType.Fighter);
          }
          else
            player.oid.LoginCreature.LevelUp(NwClass.FromClassType(ClassType.Fighter), 1);

          // On donne les autres capacités de niveau 1
          if (player.learnableSkills.TryAdd(CustomSkill.FighterSecondWind, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterSecondWind], (int)SkillSystem.Category.Class)))
            player.learnableSkills[CustomSkill.FighterSecondWind].LevelUp(player);

          // TODO : Donner le choix d'une style de combat

          break;

        case 2:

          player.oid.LoginCreature.LevelUp(NwClass.FromClassType(ClassType.Fighter), 1);

          if (player.learnableSkills.TryAdd(CustomSkill.FighterSurge, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterSurge])))
            player.learnableSkills[CustomSkill.FighterSurge].LevelUp(player);

          player.learnableSkills[CustomSkill.FighterSurge].source.Add(Category.Class);

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
