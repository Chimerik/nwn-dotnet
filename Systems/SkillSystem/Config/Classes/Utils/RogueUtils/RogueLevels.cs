using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Rogue
  {
    public static void HandleRogueLevelUp(Player player, int level, LearnableSkill playerClass)
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

            CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, 0, (int)ClassType.Rogue);
            playerClass.acquiredPoints = 0;
          }
          else
            CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, player.oid.LoginCreature.Classes.Count, (int)ClassType.Rogue);

          // On donne les autres capacités de niveau 1
          if (!player.oid.LoginCreature.KnowsFeat(Feat.SneakAttack))
            player.oid.LoginCreature.AddFeat(Feat.SneakAttack);

          if (!player.windows.TryGetValue("expertiseChoice", out var expertise)) player.windows.Add("expertiseChoice", new ExpertiseChoiceWindow(player));
          else ((ExpertiseChoiceWindow)expertise).CreateWindow();

          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Rogue;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat);

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.FighterBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterBonusAttack], player));
          player.learnableSkills[CustomSkill.FighterBonusAttack].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterBonusAttack].source.Add(Category.Class);

          player.oid.LoginCreature.BaseAttackCount += 1; 

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat);

          break;

        case 6:

          if (!player.windows.TryGetValue("featSelection", out var feat6)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat6).CreateWindow();

          break;

        case 7:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9:

          player.learnableSkills.TryAdd(CustomSkill.FighterInflexible, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterInflexible], player));
          player.learnableSkills[CustomSkill.FighterInflexible].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterInflexible].source.Add(Category.Class);

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 11:

          player.learnableSkills.TryAdd(CustomSkill.FighterBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterBonusAttack], player));
          player.learnableSkills[CustomSkill.FighterBonusAttack].LevelUp(player);

          player.oid.LoginCreature.BaseAttackCount += 1;

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 13:

          player.learnableSkills.TryAdd(CustomSkill.FighterInflexible, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterInflexible], player));
          player.learnableSkills[CustomSkill.FighterInflexible].LevelUp(player);

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 14:

          if (!player.windows.TryGetValue("featSelection", out var feat14)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat14).CreateWindow();

          break;

        case 15:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.FighterInflexible, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterInflexible], player));
          player.learnableSkills[CustomSkill.FighterInflexible].LevelUp(player);

          player.learnableSkills.TryAdd(CustomSkill.FighterSurge, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterSurge], player));
          player.learnableSkills[CustomSkill.FighterSurge].LevelUp(player);

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.FighterBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterBonusAttack], player));
          player.learnableSkills[CustomSkill.FighterBonusAttack].LevelUp(player);

          player.oid.LoginCreature.BaseAttackCount += 1;

          break;
      }
    }
  }
}
