using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static void HandleFighterLevelUp(Player player, int level, LearnableSkill playerClass)
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
          player.LearnClassSkill(CustomSkill.FighterSecondWind);

          if (!player.windows.TryGetValue("fightingStyleSelection", out var style)) player.windows.Add("fightingStyleSelection", new FightingStyleSelectionWindow(player, CustomSkill.Fighter));
          else ((FightingStyleSelectionWindow)style).CreateWindow(CustomSkill.Fighter);

          if (!player.windows.TryGetValue("expertiseDarmeSelection", out var invo1)) player.windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(player, 3));
          else ((ExpertiseDarmeSelectionWindow)invo1).CreateWindow(3);

          break;

        case 2:

          player.LearnClassSkill(CustomSkill.FighterSurge);
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FighterSurge, 1);

          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Fighter;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          if (!player.windows.TryGetValue("expertiseDarmeSelection", out var invo4)) player.windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(player));
          else ((ExpertiseDarmeSelectionWindow)invo4).CreateWindow();

          break;

        case 5:

          player.LearnClassSkill(CustomSkill.AttaqueSupplementaire);
          player.LearnClassSkill(CustomSkill.FighterAvantageTactique);

          CreatureUtils.InitializeNumAttackPerRound(player.oid.LoginCreature);

          break;

        case 6:

          if (!player.windows.TryGetValue("featSelection", out var feat6)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat6).CreateWindow();

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9: player.LearnClassSkill(CustomSkill.FighterInflexible); break;

        case 10:

          if (!player.windows.TryGetValue("expertiseDarmeSelection", out var invo10)) player.windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(player));
          else ((ExpertiseDarmeSelectionWindow)invo10).CreateWindow();

          break;

        case 11:

          player.LearnClassSkill(CustomSkill.AttaqueSupplementaire2);
          CreatureUtils.InitializeNumAttackPerRound(player.oid.LoginCreature);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 13:

          player.LearnClassSkill(CustomSkill.FighterInflexible);
          player.LearnClassSkill(CustomSkill.FighterAttaquesEtudiees);

          break;

        case 14:

          if (!player.windows.TryGetValue("featSelection", out var feat14)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat14).CreateWindow();

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          if (!player.windows.TryGetValue("expertiseDarmeSelection", out var invo16)) player.windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(player));
          else ((ExpertiseDarmeSelectionWindow)invo16).CreateWindow();

          break;

        case 17:

          player.LearnClassSkill(CustomSkill.FighterInflexible);
          player.LearnClassSkill(CustomSkill.FighterSurge);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          break;

        case 20:

          player.LearnClassSkill(CustomSkill.AttaqueSupplementaire3);
          CreatureUtils.InitializeNumAttackPerRound(player.oid.LoginCreature);

          break;
      }
    }
  }
}
