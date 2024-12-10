using System.Collections.Generic;
using Anvil.API;
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
          player.LearnClassSkill(CustomSkill.BarbarianUnarmoredDefence);
          player.LearnClassSkill(CustomSkill.BarbarianRage);

          if (!player.windows.TryGetValue("expertiseDarmeSelection", out var invo1)) player.windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(player, 2));
          else ((ExpertiseDarmeSelectionWindow)invo1).CreateWindow(2);

          break;

        case 2: player.LearnClassSkill(CustomSkill.BarbarianRecklessAttack); break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Barbarian;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          List<int> skillList = new();

          foreach (var skill in startingPackage.skillChoiceList)
            if (!player.learnableSkills.ContainsKey(skill.id))
              skillList.Add(skill.id);

          if (skillList.Count > 0)
          {
            if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 1));
            else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 1);
          }

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          if (!player.windows.TryGetValue("expertiseDarmeSelection", out var invo4)) player.windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(player));
          else ((ExpertiseDarmeSelectionWindow)invo4).CreateWindow();

          break;

        case 5:

          player.LearnClassSkill(CustomSkill.AttaqueSupplementaire);
          CreatureUtils.InitializeNumAttackPerRound(player.oid.LoginCreature);
          player.LearnClassSkill(CustomSkill.BarbarianFastMovement);

          break;

        case 7: player.LearnClassSkill(CustomSkill.BarbarianInstinctSauvage); break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9: player.LearnClassSkill(CustomSkill.FrappeBrutale); break;

        case 10:

          if (!player.windows.TryGetValue("expertiseDarmeSelection", out var invo10)) player.windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(player));
          else ((ExpertiseDarmeSelectionWindow)invo10).CreateWindow();

          break;

        case 11:

          player.LearnClassSkill(CustomSkill.BarbarianRageImplacable);
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value = 10;

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 13:
          player.LearnClassSkill(CustomSkill.FrappeSiderante);
          player.LearnClassSkill(CustomSkill.FrappeDechirante);
          break;

        case 15: player.LearnClassSkill(CustomSkill.BarbarianRagePersistante); break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 18: player.LearnClassSkill(CustomSkill.BarbarianPuissanceIndomptable); break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          break;

        case 20:

          player.LearnClassSkill(CustomSkill.BarbarianChampionPrimitif);
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Strength, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) + 4));
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) + 4));

          break;
      }
    }
  }
}
