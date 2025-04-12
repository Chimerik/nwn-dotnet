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

          player.LearnClassSkill(CustomSkill.RangerEnnemiJuré);

          if (!player.windows.TryGetValue("fightingStyleSelection", out var style)) player.windows.Add("fightingStyleSelection", new FightingStyleSelectionWindow(player, CustomSkill.Ranger));
          else ((FightingStyleSelectionWindow)style).CreateWindow(CustomSkill.Ranger);


          if (!player.windows.TryGetValue("spellSelection", out var spell)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 2));
          else ((SpellSelectionWindow)spell).CreateWindow(ClassType.Ranger, 2);

          if (!player.windows.TryGetValue("expertiseDarmeSelection", out var invo1)) player.windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(player, 2));
          else ((ExpertiseDarmeSelectionWindow)invo1).CreateWindow(2);

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks1)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks1).CreateWindow(1);

          break;

        case 2:

          if (!player.windows.TryGetValue("spellSelection", out var spell2)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell2).CreateWindow(ClassType.Ranger, 1);

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_CHOICE").Value = 1;

          if (!player.windows.TryGetValue("expertiseChoice", out var expertise3)) player.windows.Add("expertiseChoice", new ExpertiseChoiceWindow(player));
          else ((ExpertiseChoiceWindow)expertise3).CreateWindow();

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks2)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks2).CreateWindow(1);

          player.LearnClassSkill(CustomSkill.RangerExplorationHabile);

          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Ranger;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell3)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell3).CreateWindow(ClassType.Ranger, 1);

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks3)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks3).CreateWindow(1);

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell4)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell4).CreateWindow(ClassType.Ranger, 1);

          break;

        case 5:

          if (!player.windows.TryGetValue("spellSelection", out var spell5)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell5).CreateWindow(ClassType.Ranger, 1);

          player.LearnClassSkill(CustomSkill.AttaqueSupplementaire);
          CreatureUtils.InitializeNumAttackPerRound(player.oid.LoginCreature);
          
          break;

        case 6:  
          
          player.LearnClassSkill(CustomSkill.RangerVagabondage);

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks6)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks6).CreateWindow(1);

          break;

        case 7:

          if (!player.windows.TryGetValue("spellSelection", out var spell7)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell7).CreateWindow(ClassType.Ranger, 1);

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9:

          if (!player.windows.TryGetValue("spellSelection", out var spell9)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell9).CreateWindow(ClassType.Ranger, 1);

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks9)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks9).CreateWindow(1);

          break;

        case 10: player.LearnClassSkill(CustomSkill.RangerInfatiguable); break;

        case 11:

          if (!player.windows.TryGetValue("expertiseChoice", out var expertise8)) player.windows.Add("expertiseChoice", new ExpertiseChoiceWindow(player));
          else ((ExpertiseChoiceWindow)expertise8).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell11)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell11).CreateWindow(ClassType.Ranger, 1);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks12)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks12).CreateWindow(1);

          break;

        case 13:

          if (!player.windows.TryGetValue("spellSelection", out var spell13)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell13).CreateWindow(ClassType.Ranger, 1);

          break;

        case 14: 
          
          player.LearnClassSkill(CustomSkill.RangerVoileNaturel);

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks14)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks14).CreateWindow(1);

          break;

        case 15:

          if (!player.windows.TryGetValue("spellSelection", out var spell15)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell15).CreateWindow(ClassType.Ranger, 1);

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks16)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks16).CreateWindow(1);

          break;

        case 17:

          player.LearnClassSkill(CustomSkill.RangerPrecis);

          if (!player.windows.TryGetValue("spellSelection", out var spell17)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell17).CreateWindow(ClassType.Ranger, 1);

          break;
         
        case 18: 
          
          player.LearnClassSkill(CustomSkill.RangerSensSauvages);

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks18)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks18).CreateWindow(1);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell19)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 1));
          else ((SpellSelectionWindow)spell19).CreateWindow(ClassType.Ranger, 1);

          break;

        case 20: 
          
          player.LearnClassSkill(CustomSkill.RangerPourfendeur);

          if (!player.windows.TryGetValue("rangerKnacksSelection", out var knacks20)) player.windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(player, 1));
          else ((RangerKnacksSelectionWindow)knacks20).CreateWindow(1);

          break;
      }
    }
  }
}
