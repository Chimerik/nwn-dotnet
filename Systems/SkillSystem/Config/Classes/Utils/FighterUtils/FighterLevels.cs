using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static void HandleFighterLevelUp(Player player, int level)
    {
      switch (level)
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
          player.learnableSkills.TryAdd(CustomSkill.FighterSecondWind, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterSecondWind]));
          player.learnableSkills[CustomSkill.FighterSecondWind].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterSecondWind].source.Add(Category.Class);
          int chosenStyle = player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CHOSEN_FIGHTER_STYLE").Value;

          player.learnableSkills.TryAdd(chosenStyle, new LearnableSkill((LearnableSkill)learnableDictionary[chosenStyle]));
          player.learnableSkills[chosenStyle].LevelUp(player);
          player.learnableSkills[chosenStyle].source.Add(Category.Class);

          break;
        case 2:

          player.learnableSkills.TryAdd(CustomSkill.FighterSurge, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterSurge]));
          player.learnableSkills[CustomSkill.FighterSurge].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterSurge].source.Add(Category.Class);

          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Fighter;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.FighterBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterBonusAttack]));
          player.learnableSkills[CustomSkill.FighterBonusAttack].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterBonusAttack].source.Add(Category.Class);

          player.oid.LoginCreature.BaseAttackBonus += 1; break;

        case 6:

          if (!player.windows.TryGetValue("featSelection", out var feat6)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat6).CreateWindow();

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9:

          player.learnableSkills.TryAdd(CustomSkill.FighterInflexible, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterInflexible]));
          player.learnableSkills[CustomSkill.FighterInflexible].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterInflexible].source.Add(Category.Class);

          break;

        case 11:

          player.learnableSkills.TryAdd(CustomSkill.FighterBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterBonusAttack]));
          player.learnableSkills[CustomSkill.FighterBonusAttack].LevelUp(player);

          player.oid.LoginCreature.BaseAttackBonus += 1;

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 13:

          player.learnableSkills.TryAdd(CustomSkill.FighterInflexible, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterInflexible]));
          player.learnableSkills[CustomSkill.FighterInflexible].LevelUp(player);

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.FighterInflexible, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterInflexible]));
          player.learnableSkills[CustomSkill.FighterInflexible].LevelUp(player);

          player.learnableSkills.TryAdd(CustomSkill.FighterSurge, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterSurge]));
          player.learnableSkills[CustomSkill.FighterSurge].LevelUp(player);

          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.FighterBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterBonusAttack]));
          player.learnableSkills[CustomSkill.FighterBonusAttack].LevelUp(player);

          player.oid.LoginCreature.BaseAttackBonus += 1;

          break;
      }
    }
  }
}
