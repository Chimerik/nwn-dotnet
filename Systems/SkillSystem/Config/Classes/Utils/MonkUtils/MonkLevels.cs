using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Monk
  {
    public static async void HandleMonkLevelUp(Player player, int level, LearnableSkill playerClass)
    {
      switch (level)
      {
        case 1:

          // Si c'est le tout premier niveau, on donne le starting package
          if (player.oid.LoginCreature.Level == 2)
          {
            foreach (Learnable learnable in startingPackage.freeLearnables)
              player.LearnClassSkill(learnable.id);

            foreach (Learnable learnable in startingPackage.learnables)
            {
              player.learnableSkills.TryAdd(learnable.id, new LearnableSkill((LearnableSkill)learnable, player));
              player.learnableSkills[learnable.id].source.Add(Category.Class);

              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;
            }

            playerClass.acquiredPoints = 0;
          }

          // On donne les autres capacités de niveau 1
          player.LearnClassSkill(CustomSkill.MonkUnarmoredDefence);
          player.LearnClassSkill(CustomSkill.MonkBonusAttack);

          break;

        case 2:

          player.LearnClassSkill(CustomSkill.MonkPatience);
          player.LearnClassSkill(CustomSkill.MonkDelugeDeCoups);
          player.LearnClassSkill(CustomSkill.MonkUnarmoredSpeed);
          player.LearnClassSkill(CustomSkill.MonkMetabolismeSurnaturel);

          break;

        case 3:

          player.LearnClassSkill(CustomSkill.MonkParade);

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Monk;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          break;

        case 4:

          player.LearnClassSkill(CustomSkill.MonkSlowFall);

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          break;

        case 5:

          player.LearnClassSkill(CustomSkill.MonkStunStrike);
          player.LearnClassSkill(CustomSkill.AttaqueSupplementaire);

          break;

        case 6:

          //player.LearnClassSkill(CustomSkill.MonkFrappesRenforcees);

          break;

        case 7: 
          
          player.oid.LoginCreature.AddFeat(Feat.ImprovedEvasion);
          player.LearnClassSkill(CustomSkill.MonkSerenity);

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 10:

          player.oid.LoginCreature.AddFeat(Feat.PurityOfBody);
          player.oid.LoginCreature.AddFeat(Feat.DiamondBody);

          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.PuretePhysique));

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 14:

          player.LearnClassSkill(CustomSkill.ConstitutionSavesProficiency);
          player.LearnClassSkill(CustomSkill.IntelligenceSavesProficiency);
          player.LearnClassSkill(CustomSkill.WisdomSavesProficiency);
          player.LearnClassSkill(CustomSkill.CharismaSavesProficiency);
          player.LearnClassSkill(CustomSkill.MonkDiamondSoul);

          break;

        case 15:

          player.oid.OnCombatStatusChange -= MonkUtils.OnCombatMonkRecoverKi;
          player.oid.OnCombatStatusChange += MonkUtils.OnCombatMonkRecoverKi;

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 18: player.LearnClassSkill(CustomSkill.MonkDesertion); break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          break;

        case 20:

          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Dexterity, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) + 4));
          player.oid.LoginCreature.SetsRawAbilityScore(Ability.Wisdom, (byte)(player.oid.LoginCreature.GetRawAbilityScore(Ability.Wisdom) + 4));

          break;
      }

      await NwTask.NextFrame();
      OnLearnUnarmoredSpeed(player, CustomSkill.MonkUnarmoredSpeed);
    }
  }
}
