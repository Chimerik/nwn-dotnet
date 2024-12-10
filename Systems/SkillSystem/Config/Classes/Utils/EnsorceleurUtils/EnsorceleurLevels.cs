using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Ensorceleur
  {
    public static void HandleEnsorceleurLevelUp(Player player, int level, LearnableSkill playerClass)
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

          if (!player.windows.TryGetValue("cantripSelection", out var cantrip1)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Sorcerer, 4));
          else ((CantripSelectionWindow)cantrip1).CreateWindow(ClassType.Sorcerer, 4);

          if (!player.windows.TryGetValue("spellSelection", out var spell1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 2));
          else ((SpellSelectionWindow)spell1).CreateWindow(ClassType.Sorcerer, 2);

          player.LearnClassSkill(CustomSkill.SorcellerieInnee);

          break;

        case 2:

          if (!player.windows.TryGetValue("spellSelection", out var spell2)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 2));
          else ((SpellSelectionWindow)spell2).CreateWindow(ClassType.Sorcerer, 2);
          
          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Ensorceleur;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell3)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 2));
          else ((SpellSelectionWindow)spell3).CreateWindow(ClassType.Sorcerer, 2);

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          if (!player.windows.TryGetValue("cantripSelection", out var cantrip4)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((CantripSelectionWindow)cantrip4).CreateWindow(ClassType.Sorcerer, 1);

          if (!player.windows.TryGetValue("spellSelection", out var spell4)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell4).CreateWindow(ClassType.Sorcerer, 1);

          break;

        case 5:

          if (!player.windows.TryGetValue("spellSelection", out var spell5)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 2));
          else ((SpellSelectionWindow)spell5).CreateWindow(ClassType.Sorcerer, 2);

          player.LearnClassSkill(CustomSkill.RetablissementSorcier);

          break;

        case 6:

          if (!player.windows.TryGetValue("spellSelection", out var spell6)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell6).CreateWindow(ClassType.Sorcerer, 1);

          break;

        case 7:

          if (!player.windows.TryGetValue("spellSelection", out var spell7)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell7).CreateWindow(ClassType.Sorcerer, 1);

          player.LearnClassSkill(CustomSkill.SorcellerieIncarnee);

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell8)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell8).CreateWindow(ClassType.Sorcerer, 1);

          break;

        case 9:

          if (!player.windows.TryGetValue("spellSelection", out var spell9)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 2));
          else ((SpellSelectionWindow)spell9).CreateWindow(ClassType.Sorcerer, 2);

          break;

        case 10:

          if (!player.windows.TryGetValue("cantripSelection", out var cantrip10)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((CantripSelectionWindow)cantrip10).CreateWindow(ClassType.Sorcerer, 1);

          if (!player.windows.TryGetValue("spellSelection", out var spell10)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell10).CreateWindow(ClassType.Sorcerer, 1);

          player.LearnClassSkill(CustomSkill.ClercInterventionDivine);

          break;

        case 11:

          if (!player.windows.TryGetValue("spellSelection", out var spell11)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell11).CreateWindow(ClassType.Sorcerer, 1);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 13:

          if (!player.windows.TryGetValue("spellSelection", out var spell13)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell13).CreateWindow(ClassType.Sorcerer, 1);

          break;

        case 15:

          if (!player.windows.TryGetValue("spellSelection", out var spell15)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell15).CreateWindow(ClassType.Sorcerer, 1);

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 17:

          if (!player.windows.TryGetValue("spellSelection", out var spell17)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell17).CreateWindow(ClassType.Sorcerer, 1);

          break;

        case 18:

          if (!player.windows.TryGetValue("spellSelection", out var spell18)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell18).CreateWindow(ClassType.Sorcerer, 1);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell19)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell19).CreateWindow(ClassType.Sorcerer, 1);

          break;

        case 20:

          if (!player.windows.TryGetValue("spellSelection", out var spell20)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Sorcerer, 1));
          else ((SpellSelectionWindow)spell20).CreateWindow(ClassType.Sorcerer, 1);

          player.LearnClassSkill(CustomSkill.EnsoApotheose);

          break;
      }
      
      EnsoUtils.RestoreSorcerySource(player.oid.LoginCreature);
    }
  }
}
