﻿using Anvil.API;
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
          if (!player.oid.LoginCreature.KnowsFeat(Feat.SneakAttack))
            player.oid.LoginCreature.AddFeat(Feat.SneakAttack);

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

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat);

          player.learnableSkills.TryAdd(CustomSkill.ChasseurEsquiveInstinctive, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ChasseurEsquiveInstinctive], player));
          player.learnableSkills[CustomSkill.ChasseurEsquiveInstinctive].LevelUp(player);
          player.learnableSkills[CustomSkill.ChasseurEsquiveInstinctive].source.Add(Category.Class);

          break;

        case 6:

          if (!player.windows.TryGetValue("expertiseChoice", out var expertise6)) player.windows.Add("expertiseChoice", new ExpertiseChoiceWindow(player));
          else ((ExpertiseChoiceWindow)expertise6).CreateWindow();

          break;

        case 7:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat);

          player.learnableSkills.TryAdd(CustomSkill.EsquiveSurnaturelle, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EsquiveSurnaturelle], player));
          player.learnableSkills[CustomSkill.EsquiveSurnaturelle].LevelUp(player);
          player.learnableSkills[CustomSkill.EsquiveSurnaturelle].source.Add(Category.Class);

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 10:

          if (!player.windows.TryGetValue("featSelection", out var feat10)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat10).CreateWindow();

          break;

        case 11:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 13:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 14: 

          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.perceptionAveugleAura);

          break;

        case 15:

          player.learnableSkills.TryAdd(CustomSkill.WisdomSavesProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WisdomSavesProficiency], player));
          player.learnableSkills[CustomSkill.WisdomSavesProficiency].LevelUp(player);
          player.learnableSkills[CustomSkill.WisdomSavesProficiency].source.Add(Category.Class);

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 16:

          if(!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 17:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          break;

        case 19:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat.SuccessorFeat);

          if(!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          break;
      }
    }
  }
}
