using System.Linq;
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

          foreach (var spell in NwRuleset.Spells.Where(s => 0 < s.GetSpellLevelForClass(ClassType.Druid) && s.GetSpellLevelForClass(ClassType.Druid) < 10))
          {
            if (Utils.In(spell.Id, CustomSpell.RayonEmpoisonne, (int)Spell.AcidSplash, (int)Spell.Web, (int)Spell.RayOfFrost, (int)Spell.ElectricJolt, (int)Spell.Sleep,
              CustomSpell.FouleeBrumeuse, (int)Spell.BurningHands, (int)Spell.GhostlyVisage, CustomSpell.FireBolt, (int)Spell.Fireball, (int)Spell.LightningBolt,
              (int)Spell.StinkingCloud))
              continue;// ces sorts ne font pas partie du package de druide mais peuvent être appris via le cercle

            if (player.learnableSpells.TryGetValue(spell.Id, out var learnable))
            {
              learnable.learntFromClasses.Add((int)ClassType.Druid);

              if (learnable.currentLevel < 1)
                learnable.LevelUp(player);
            }
            else
            {
              LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[spell.Id], (int)ClassType.Druid);
              player.learnableSpells.Add(learnableSpell.id, learnableSpell);
              learnableSpell.LevelUp(player);
            }
          }

          // On donne les autres capacités de niveau 1

          player.learnableSkills.TryAdd(CustomSkill.Druidique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Druidique], player));
          player.learnableSkills[CustomSkill.Druidique].LevelUp(player);
          player.learnableSkills[CustomSkill.Druidique].source.Add(Category.Class);

          player.LearnAlwaysPreparedSpell(CustomSpell.SpeakAnimal, CustomClass.Druid);

          if (!player.windows.TryGetValue("ordrePrimordialSelection", out var ordrePrimordial)) player.windows.Add("ordrePrimordialSelection", new OrdrePrimordialSelectionWindow(player));
          else ((OrdrePrimordialSelectionWindow)ordrePrimordial).CreateWindow();

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageBlaireau, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageBlaireau], player));
          player.learnableSkills[CustomSkill.FormeSauvageBlaireau].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageBlaireau].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageChat, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageChat], player));
          player.learnableSkills[CustomSkill.FormeSauvageChat].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageChat].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageAraignee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageAraignee], player));
          player.learnableSkills[CustomSkill.FormeSauvageAraignee].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageAraignee].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageLoup, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageLoup], player));
          player.learnableSkills[CustomSkill.FormeSauvageLoup].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageLoup].source.Add(Category.Class);

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

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageRothe, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageRothe], player));
          player.learnableSkills[CustomSkill.FormeSauvageRothe].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageRothe].source.Add(Category.Class);

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.DruideReveilSauvage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DruideReveilSauvage], player));
          player.learnableSkills[CustomSkill.DruideReveilSauvage].LevelUp(player);
          player.learnableSkills[CustomSkill.DruideReveilSauvage].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvagePanthere, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvagePanthere], player));
          player.learnableSkills[CustomSkill.FormeSauvagePanthere].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvagePanthere].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageOursHibou, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageOursHibou], player));
          player.learnableSkills[CustomSkill.FormeSauvageOursHibou].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageOursHibou].source.Add(Category.Class);

          break;

        case 7:

          if (!player.windows.TryGetValue("fureurElementaireSelection", out var fureur)) player.windows.Add("fureurElementaireSelection", new FureurElementaireSelectionWindow(player));
          else ((FureurElementaireSelectionWindow)fureur).CreateWindow();

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 10:

          if (!player.windows.TryGetValue("spellSelection", out var spell10)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Druid, 1));
          else ((SpellSelectionWindow)spell10).CreateWindow(ClassType.Druid, 1);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageDilophosaure, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageDilophosaure], player));
          player.learnableSkills[CustomSkill.FormeSauvageDilophosaure].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageDilophosaure].source.Add(Category.Class);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 18:

          player.learnableSkills.TryAdd(CustomSkill.DruideIncantationBestiale, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DruideIncantationBestiale], player));
          player.learnableSkills[CustomSkill.DruideIncantationBestiale].LevelUp(player);
          player.learnableSkills[CustomSkill.DruideIncantationBestiale].source.Add(Category.Class);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();


          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvagePersistante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvagePersistante], player));
          player.learnableSkills[CustomSkill.FormeSauvagePersistante].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvagePersistante].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MageNature, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MageNature], player));
          player.learnableSkills[CustomSkill.MageNature].LevelUp(player);
          player.learnableSkills[CustomSkill.MageNature].source.Add(Category.Class);

          break;
      }

      DruideUtils.RestoreFormeSauvage(player.oid.LoginCreature);
    }
  }
}
