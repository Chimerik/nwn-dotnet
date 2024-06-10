using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Ranger
  {
    public static void HandleRangerLevelUp(Player player, int level, LearnableSkill playerClass)
    {
      switch (level)
      {
        case 1:

          // Si c'est le tout premier niveau, on donne le starting package
          if (player.oid.LoginCreature.Level == 2)
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

            playerClass.acquiredPoints = 0;
          }
 
          // On donne les autres capacités de niveau 1

          if (!player.windows.TryGetValue("rangerArchetypeSelection", out var archetype)) player.windows.Add("rangerArchetypeSelection", new RangerArchetypeSelectionWindow(player));
          else ((RangerArchetypeSelectionWindow)archetype).CreateWindow();

          break;

        case 2:

          if (!player.windows.TryGetValue("fightingStyleSelection", out var style)) player.windows.Add("fightingStyleSelection", new FightingStyleSelectionWindow(player, CustomSkill.Ranger));
          else ((FightingStyleSelectionWindow)style).CreateWindow(CustomSkill.Ranger);

          if (!player.windows.TryGetValue("spellSelection", out var spell2)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 2));
          else ((SpellSelectionWindow)spell2).CreateWindow(ClassType.Ranger, 0, 2);

          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Ranger;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell3)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 1));
          else ((SpellSelectionWindow)spell3).CreateWindow(ClassType.Ranger, 0, 1);

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          break;

        case 5:

          if (!player.windows.TryGetValue("spellSelection", out var spell5)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 1));
          else ((SpellSelectionWindow)spell5).CreateWindow(ClassType.Ranger, 0, 1);

          player.learnableSkills.TryAdd(CustomSkill.RangerBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RangerBonusAttack], player));
          player.learnableSkills[CustomSkill.RangerBonusAttack].LevelUp(player);
          player.learnableSkills[CustomSkill.RangerBonusAttack].source.Add(Category.Class);

          player.oid.LoginCreature.BaseAttackCount += 1; break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.RangerGreaterFavoredEnemy, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RangerGreaterFavoredEnemy], player));
          player.learnableSkills[CustomSkill.RangerGreaterFavoredEnemy].LevelUp(player);
          player.learnableSkills[CustomSkill.RangerGreaterFavoredEnemy].source.Add(Category.Class);

          if (!player.windows.TryGetValue("favoredEnemySelection", out var favoredEnemy)) player.windows.Add("favoredEnemySelection", new FavoredEnemySelectionWindow(player));
          else ((FavoredEnemySelectionWindow)favoredEnemy).CreateWindow();

          break;

        case 7:

          if (!player.windows.TryGetValue("spellSelection", out var spell7)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 1));
          else ((SpellSelectionWindow)spell7).CreateWindow(ClassType.Ranger, 0, 1);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.RangerDeplacementFluide, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RangerDeplacementFluide], player));
          player.learnableSkills[CustomSkill.RangerDeplacementFluide].LevelUp(player);
          player.learnableSkills[CustomSkill.RangerDeplacementFluide].source.Add(Category.Class);

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9:

          if (!player.windows.TryGetValue("spellSelection", out var spell9)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 1));
          else ((SpellSelectionWindow)spell9).CreateWindow(ClassType.Ranger, 0, 1);

          break;

        case 11:

          if (!player.windows.TryGetValue("spellSelection", out var spell11)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 1));
          else ((SpellSelectionWindow)spell11).CreateWindow(ClassType.Ranger, 0, 1);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 13:

          if (!player.windows.TryGetValue("spellSelection", out var spell13)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 1));
          else ((SpellSelectionWindow)spell13).CreateWindow(ClassType.Ranger, 0, 1);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.RangerDisparition, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RangerDisparition], player));
          player.learnableSkills[CustomSkill.RangerDisparition].LevelUp(player);
          player.learnableSkills[CustomSkill.RangerDisparition].source.Add(Category.Class);

          break;

        case 15:

          if (!player.windows.TryGetValue("spellSelection", out var spell15)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 1));
          else ((SpellSelectionWindow)spell15).CreateWindow(ClassType.Ranger, 0, 1);

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 17:

          if (!player.windows.TryGetValue("spellSelection", out var spell17)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 1));
          else ((SpellSelectionWindow)spell17).CreateWindow(ClassType.Ranger, 0, 1);

          break;
         
        case 18:

          player.learnableSkills.TryAdd(CustomSkill.RangerSensSauvages, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RangerSensSauvages], player));
          player.learnableSkills[CustomSkill.RangerSensSauvages].LevelUp(player);
          player.learnableSkills[CustomSkill.RangerSensSauvages].source.Add(Category.Class);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell19)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 0, 1));
          else ((SpellSelectionWindow)spell19).CreateWindow(ClassType.Ranger, 0, 1);

          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.RangerTueurImplacable, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RangerTueurImplacable], player));
          player.learnableSkills[CustomSkill.RangerTueurImplacable].LevelUp(player);
          player.learnableSkills[CustomSkill.RangerTueurImplacable].source.Add(Category.Class);

          break;
      }
    }
  }
}
