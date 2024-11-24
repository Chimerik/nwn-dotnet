using Anvil.API;
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

          player.LearnClassSkill(CustomSkill.RoublardViseeStable);

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          break;

        case 5:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat);

          player.LearnClassSkill(CustomSkill.EsquiveInstinctive);
          player.LearnClassSkill(CustomSkill.RoublardFrappeRusee);

          break;

        case 6:

          if (!player.windows.TryGetValue("expertiseChoice", out var expertise6)) player.windows.Add("expertiseChoice", new ExpertiseChoiceWindow(player));
          else ((ExpertiseChoiceWindow)expertise6).CreateWindow();

          break;

        case 7:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.SneakAttack).SuccessorFeat.SuccessorFeat.SuccessorFeat);

          player.LearnClassSkill(CustomSkill.EsquiveSurnaturelle);
          player.LearnClassSkill(CustomSkill.RoublardSavoirFaire);

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

          player.LearnClassSkill(CustomSkill.RoublardFrappeRuseeAmelioree);

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

          //player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.perceptionAveugleAura);

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.FrappeRuseeEffectTag);

          player.learnableSkills.Remove(CustomSkill.RoublardFrappeRusee);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.RoublardFrappeRusee);
          player.LearnClassSkill(CustomSkill.RoublardFrappePerfide);

          break;

        case 15:

          player.LearnClassSkill(CustomSkill.WisdomSavesProficiency);
          player.LearnClassSkill(CustomSkill.CharismaSavesProficiency);

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

        case 20: player.LearnClassSkill(CustomSkill.RoublardCoupDeChance); break;
      }
    }
  }
}
