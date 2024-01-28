using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Barbarian
  {
    public static void HandleBarbarianLevelUp(Player player, int level, LearnableSkill playerClass)
    {
      switch (level)
      {
        case 1:

          // Si c'est le tout premier niveau, on donne le starting package
          if (player.oid.LoginCreature.Level < 2)
          {
            foreach (Learnable learnable in startingPackage.freeLearnables)
            {
              if (player.learnableSkills.TryAdd(learnable.id, new LearnableSkill((LearnableSkill)learnable, player)))
                player.learnableSkills[learnable.id].LevelUp(player);

              player.learnableSkills[learnable.id].source.Add(Category.Class);
            }

            foreach (Learnable learnable in startingPackage.learnables)
            {
              player.learnableSkills.TryAdd(learnable.id, new LearnableSkill((LearnableSkill)learnable, player));
              player.learnableSkills[learnable.id].source.Add(Category.Class);

              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;
            }

            CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, 0, (int)ClassType.Barbarian);
            playerClass.acquiredPoints = 0;
          }
          else
            CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, player.oid.LoginCreature.Classes.Count, (int)ClassType.Barbarian);

          // On donne les autres capacités de niveau 1
          player.learnableSkills.TryAdd(CustomSkill.BarbarianUnarmoredDefence, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianUnarmoredDefence], player));
          player.learnableSkills[CustomSkill.BarbarianUnarmoredDefence].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianUnarmoredDefence].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.BarbarianRage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianRage], player));
          player.learnableSkills[CustomSkill.BarbarianRage].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianRage].source.Add(Category.Class);

          break;

        case 3:

          /*player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Barbarian;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();*/

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.BarbarianBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianBonusAttack], player));
          player.learnableSkills[CustomSkill.BarbarianBonusAttack].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianBonusAttack].source.Add(Category.Class);

          player.oid.LoginCreature.BaseAttackCount += 1;

          player.learnableSkills.TryAdd(CustomSkill.BarbarianFastMovement, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianFastMovement], player));
          player.learnableSkills[CustomSkill.BarbarianFastMovement].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianFastMovement].source.Add(Category.Class);

          break;

        case 7:

          player.learnableSkills.TryAdd(CustomSkill.BarbarianInstinctSauvage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianInstinctSauvage], player));
          player.learnableSkills[CustomSkill.BarbarianInstinctSauvage].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianInstinctSauvage].source.Add(Category.Class);

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9:

          player.learnableSkills.TryAdd(CustomSkill.BarbarianCritiqueBrutal, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianCritiqueBrutal], player));
          player.learnableSkills[CustomSkill.BarbarianCritiqueBrutal].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianCritiqueBrutal].source.Add(Category.Class);

          break;

        case 11:

          player.learnableSkills.TryAdd(CustomSkill.BarbarianRageImplacable, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianRageImplacable], player));
          player.learnableSkills[CustomSkill.BarbarianRageImplacable].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianRageImplacable].source.Add(Category.Class);

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value = 10;

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 15:

          player.learnableSkills.TryAdd(CustomSkill.BarbarianRagePersistante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianRagePersistante], player));
          player.learnableSkills[CustomSkill.BarbarianRagePersistante].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianRagePersistante].source.Add(Category.Class);

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 18:

          player.learnableSkills.TryAdd(CustomSkill.BarbarianPuissanceIndomptable, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianPuissanceIndomptable], player));
          player.learnableSkills[CustomSkill.BarbarianPuissanceIndomptable].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianPuissanceIndomptable].source.Add(Category.Class);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.BarbarianChampionPrimitif, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BarbarianChampionPrimitif], player));
          player.learnableSkills[CustomSkill.BarbarianChampionPrimitif].LevelUp(player);
          player.learnableSkills[CustomSkill.BarbarianChampionPrimitif].source.Add(Category.Class);

          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Strength, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) + 4));
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) + 4));

          break;
      }
    }
  }
}
