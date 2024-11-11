using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Occultiste
  {
    public static void HandleOccultisteLevelUp(Player player, int level, LearnableSkill playerClass)
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

          if (!player.windows.TryGetValue("spellSelection", out var cantrip1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, 2, 2));
          else ((SpellSelectionWindow)cantrip1).CreateWindow((ClassType)CustomClass.Occultiste, 2, 2);

          if (!player.windows.TryGetValue("invocationOcculteSelection", out var invo1)) player.windows.Add("invocationOcculteSelection", new InvocationOcculteSelectionWindow(player, 1));
          else ((InvocationOcculteSelectionWindow)invo1).CreateWindow(1);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.OccultisteFourberieMagique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.OccultisteFourberieMagique], player));
          player.learnableSkills[CustomSkill.OccultisteFourberieMagique].LevelUp(player);
          player.learnableSkills[CustomSkill.OccultisteFourberieMagique].source.Add(Category.Class);

          if (!player.windows.TryGetValue("spellSelection", out var spell2)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells:1));
          else ((SpellSelectionWindow)spell2).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          if (!player.windows.TryGetValue("invocationOcculteSelection", out var invo2)) player.windows.Add("invocationOcculteSelection", new InvocationOcculteSelectionWindow(player, 2));
          else ((InvocationOcculteSelectionWindow)invo2).CreateWindow(2);

          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Occultiste;

          if (!player.windows.TryGetValue("subClassSelection", out var subClass)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)subClass).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell3)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell3).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var cantrip4)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, 1, 1));
          else ((SpellSelectionWindow)cantrip4).CreateWindow((ClassType)CustomClass.Occultiste, 1, 1);


          break;

        case 5:

          if (!player.windows.TryGetValue("spellSelection", out var spell5)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell5).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          if (!player.windows.TryGetValue("invocationOcculteSelection", out var invo5)) player.windows.Add("invocationOcculteSelection", new InvocationOcculteSelectionWindow(player, 2));
          else ((InvocationOcculteSelectionWindow)invo5).CreateWindow(2);

          break;

        case 6:

          if (!player.windows.TryGetValue("spellSelection", out var spell6)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell6).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          break;

        case 7:

          if (!player.windows.TryGetValue("spellSelection", out var spell7)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell7).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          if (!player.windows.TryGetValue("invocationOcculteSelection", out var invo7)) player.windows.Add("invocationOcculteSelection", new InvocationOcculteSelectionWindow(player, 1));
          else ((InvocationOcculteSelectionWindow)invo7).CreateWindow(1);

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell8)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell8).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          break;

        case 9:

          if (!player.windows.TryGetValue("spellSelection", out var spell9)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell9).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          if (!player.windows.TryGetValue("invocationOcculteSelection", out var invo9)) player.windows.Add("invocationOcculteSelection", new InvocationOcculteSelectionWindow(player, 1));
          else ((InvocationOcculteSelectionWindow)invo9).CreateWindow(1);

          break;

        case 10:

          if (!player.windows.TryGetValue("spellSelection", out var cantrip10)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, 1));
          else ((SpellSelectionWindow)cantrip10).CreateWindow((ClassType)CustomClass.Occultiste, 1);

          break;

        case 11:

          if (!player.windows.TryGetValue("spellSelection", out var spell11)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell11).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          if (!player.windows.TryGetValue("arcaneMystiqueSelection", out var arcane11)) player.windows.Add("arcaneMystiqueSelection", new ArcaneMystiqueSelectionWindow(player, 6));
          else ((ArcaneMystiqueSelectionWindow)arcane11).CreateWindow(6);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          if (!player.windows.TryGetValue("invocationOcculteSelection", out var invo12)) player.windows.Add("invocationOcculteSelection", new InvocationOcculteSelectionWindow(player, 1));
          else ((InvocationOcculteSelectionWindow)invo12).CreateWindow(1);

          break;

        case 13:
          
          if (!player.windows.TryGetValue("spellSelection", out var spell13)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell13).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          if (!player.windows.TryGetValue("arcaneMystiqueSelection", out var arcane13)) player.windows.Add("arcaneMystiqueSelection", new ArcaneMystiqueSelectionWindow(player, 7));
          else ((ArcaneMystiqueSelectionWindow)arcane13).CreateWindow(7);

          break;

        case 15:

          if (!player.windows.TryGetValue("spellSelection", out var spell15)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell15).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          if (!player.windows.TryGetValue("arcaneMystiqueSelection", out var arcane15)) player.windows.Add("arcaneMystiqueSelection", new ArcaneMystiqueSelectionWindow(player, 8));
          else ((ArcaneMystiqueSelectionWindow)arcane15).CreateWindow(8);

          if (!player.windows.TryGetValue("invocationOcculteSelection", out var invo15)) player.windows.Add("invocationOcculteSelection", new InvocationOcculteSelectionWindow(player, 1));
          else ((InvocationOcculteSelectionWindow)invo15).CreateWindow(1);

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 17:

          if (!player.windows.TryGetValue("spellSelection", out var spell17)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell17).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          if (!player.windows.TryGetValue("arcaneMystiqueSelection", out var arcane17)) player.windows.Add("arcaneMystiqueSelection", new ArcaneMystiqueSelectionWindow(player, 9));
          else ((ArcaneMystiqueSelectionWindow)arcane17).CreateWindow(9);

          break;

        case 18:

          if (!player.windows.TryGetValue("invocationOcculteSelection", out var invo18)) player.windows.Add("invocationOcculteSelection", new InvocationOcculteSelectionWindow(player, 1));
          else ((InvocationOcculteSelectionWindow)invo18).CreateWindow(1);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell19)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.Occultiste, nbSpells: 1));
          else ((SpellSelectionWindow)spell19).CreateWindow((ClassType)CustomClass.Occultiste, nbSpells: 1);

          break;

        case 20:


          break;
      }
    }
  }
}
