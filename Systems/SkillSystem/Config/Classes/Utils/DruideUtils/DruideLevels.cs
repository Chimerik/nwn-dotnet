using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Druide
  {
    public static void HandleDruideLevelUp(Player player, int level, LearnableSkill playerClass)
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

          if (!player.windows.TryGetValue("spellSelection", out var spell1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Druid, 2));
          else ((SpellSelectionWindow)spell1).CreateWindow(ClassType.Druid, 2);

          player.learnableSkills.TryAdd(CustomSkill.Druidique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Druidique], player));
          player.learnableSkills[CustomSkill.Druidique].LevelUp(player);
          player.learnableSkills[CustomSkill.Druidique].source.Add(Category.Class);

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.SpeakAnimal, CustomClass.Druid);

          if (!player.windows.TryGetValue("ordrePrimordialSelection", out var ordrePrimordial)) player.windows.Add("ordrePrimordialSelection", new OrdrePrimordialSelectionWindow(player));
          else ((OrdrePrimordialSelectionWindow)ordrePrimordial).CreateWindow();

          break;

        case 2:


          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Druide;

          if (!player.windows.TryGetValue("subClassSelection", out var subClass)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)subClass).CreateWindow();

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          if (!player.windows.TryGetValue("spellSelection", out var spell4)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Druid, 1));
          else ((SpellSelectionWindow)spell4).CreateWindow(ClassType.Druid, 1);

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.RetablissementSorcier, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RetablissementSorcier], player));
          player.learnableSkills[CustomSkill.RetablissementSorcier].LevelUp(player);
          player.learnableSkills[CustomSkill.RetablissementSorcier].source.Add(Category.Class);

          break;

        case 6:


          break;

        case 7:

          player.learnableSkills.TryAdd(CustomSkill.SorcellerieIncarnee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SorcellerieIncarnee], player));
          player.learnableSkills[CustomSkill.SorcellerieIncarnee].LevelUp(player);
          player.learnableSkills[CustomSkill.SorcellerieIncarnee].source.Add(Category.Class);

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9:


          break;

        case 10:

          if (!player.windows.TryGetValue("spellSelection", out var spell10)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Druid, 1));
          else ((SpellSelectionWindow)spell10).CreateWindow(ClassType.Druid, 1);

          player.learnableSkills.TryAdd(CustomSkill.ClercInterventionDivine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercInterventionDivine], player));
          player.learnableSkills[CustomSkill.ClercInterventionDivine].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercInterventionDivine].source.Add(Category.Class);

          break;

        case 11:


          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 13:


          break;

        case 15:



          break;

        case 16:



          break;

        case 17:



          break;

        case 18:



          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();


          break;

        case 20:

          if (!player.windows.TryGetValue("spellSelection", out var spell20)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Cleric, 3, 0));
          else ((SpellSelectionWindow)spell20).CreateWindow(ClassType.Sorcerer, 0, 1);

          player.learnableSkills.TryAdd(CustomSkill.EnsoApotheose, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoApotheose], player));
          player.learnableSkills[CustomSkill.EnsoApotheose].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoApotheose].source.Add(Category.Class);

          break;
      }

      DruideUtils.RestoreFormeSauvage(player.oid.LoginCreature);
    }
  }
}
