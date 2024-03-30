using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Monk
  {
    public static void HandleMonkLevelUp(Player player, int level, LearnableSkill playerClass)
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
          player.learnableSkills.TryAdd(CustomSkill.MonkUnarmoredDefence, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkUnarmoredDefence], player));
          player.learnableSkills[CustomSkill.MonkUnarmoredDefence].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkUnarmoredDefence].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkBonusAttack], player));
          player.learnableSkills[CustomSkill.MonkBonusAttack].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkBonusAttack].source.Add(Category.Class);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.MonkPatience, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkPatience], player));
          player.learnableSkills[CustomSkill.MonkPatience].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkPatience].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkDelugeDeCoups, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkDelugeDeCoups], player));
          player.learnableSkills[CustomSkill.MonkDelugeDeCoups].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkDelugeDeCoups].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkUnarmoredSpeed, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkUnarmoredSpeed], player));
          player.learnableSkills[CustomSkill.MonkUnarmoredSpeed].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkUnarmoredSpeed].source.Add(Category.Class);

          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Monk;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          break;

        case 4:

          player.learnableSkills.TryAdd(CustomSkill.MonkSlowFall, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkSlowFall], player));
          player.learnableSkills[CustomSkill.MonkSlowFall].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkSlowFall].source.Add(Category.Class);

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          break;

        case 5:

          player.oid.LoginCreature.BaseAttackCount += 1;

          player.learnableSkills.TryAdd(CustomSkill.MonkStunStrike, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkStunStrike], player));
          player.learnableSkills[CustomSkill.MonkStunStrike].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkStunStrike].source.Add(Category.Class);

          break;

        case 7: 
          
          player.oid.LoginCreature.AddFeat(Feat.ImprovedEvasion);

          player.learnableSkills.TryAdd(CustomSkill.MonkSerenity, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkSerenity], player));
          player.learnableSkills[CustomSkill.MonkSerenity].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkSerenity].source.Add(Category.Class);

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 10:

          player.oid.LoginCreature.AddFeat(Feat.PurityOfBody);
          player.oid.LoginCreature.AddFeat(Feat.DiamondBody);

          player.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity((IPDamageType)CustomItemPropertiesDamageType.Poison, IPDamageImmunityType.Immunity100Pct), EffectDuration.Permanent);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 14:

          if (player.learnableSkills.TryAdd(CustomSkill.ConstitutionSavesProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ConstitutionSavesProficiency], player)))
          {
            player.learnableSkills[CustomSkill.ConstitutionSavesProficiency].LevelUp(player);
            player.learnableSkills[CustomSkill.ConstitutionSavesProficiency].source.Add(Category.Class);
          }

          if (player.learnableSkills.TryAdd(CustomSkill.IntelligenceSavesProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.IntelligenceSavesProficiency], player)))
          {
            player.learnableSkills[CustomSkill.IntelligenceSavesProficiency].LevelUp(player);
            player.learnableSkills[CustomSkill.IntelligenceSavesProficiency].source.Add(Category.Class);
          }

          if (player.learnableSkills.TryAdd(CustomSkill.WisdomSavesProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WisdomSavesProficiency], player)))
          {
            player.learnableSkills[CustomSkill.WisdomSavesProficiency].LevelUp(player);
            player.learnableSkills[CustomSkill.WisdomSavesProficiency].source.Add(Category.Class);
          }

          if (player.learnableSkills.TryAdd(CustomSkill.CharismaSavesProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.CharismaSavesProficiency], player)))
          {
            player.learnableSkills[CustomSkill.CharismaSavesProficiency].LevelUp(player);
            player.learnableSkills[CustomSkill.CharismaSavesProficiency].source.Add(Category.Class);
          }

          player.learnableSkills.TryAdd(CustomSkill.MonkDiamondSoul, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkDiamondSoul], player));
          player.learnableSkills[CustomSkill.MonkDiamondSoul].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkDiamondSoul].source.Add(Category.Class);

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 18:

          player.learnableSkills.TryAdd(CustomSkill.MonkDesertion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkDesertion], player));
          player.learnableSkills[CustomSkill.MonkDesertion].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkDesertion].source.Add(Category.Class);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          break;

        case 20:

          player.oid.OnCombatStatusChange -= MonkUtils.OnCombatMonkRecoverKi;
          player.oid.OnCombatStatusChange += MonkUtils.OnCombatMonkRecoverKi;

          break;
      }

      MonkUtils.RestoreKi(player.oid.LoginCreature);
      OnLearnUnarmoredSpeed(player, CustomSkill.MonkUnarmoredSpeed);
    }
  }
}
