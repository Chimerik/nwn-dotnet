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
              (int)Spell.StinkingCloud, CustomSpell.AppelDeFamilier))
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

          player.LearnClassSkill(CustomSkill.Druidique);
          player.LearnAlwaysPreparedSpell(CustomSpell.SpeakAnimal, CustomClass.Druid);

          if (!player.windows.TryGetValue("ordrePrimordialSelection", out var ordrePrimordial)) player.windows.Add("ordrePrimordialSelection", new OrdrePrimordialSelectionWindow(player));
          else ((OrdrePrimordialSelectionWindow)ordrePrimordial).CreateWindow();

          break;

        case 2:
          player.LearnClassSkill(CustomSkill.FormeSauvageBlaireau);
          player.LearnClassSkill(CustomSkill.FormeSauvageChat);
          player.LearnClassSkill(CustomSkill.FormeSauvageAraignee);
          player.LearnClassSkill(CustomSkill.FormeSauvageLoup);
          player.LearnClassSkill(CustomSkill.DruideCompagnonSauvage);
          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Druide;

          if (!player.windows.TryGetValue("subClassSelection", out var subClass)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)subClass).CreateWindow();

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          if (!player.windows.TryGetValue("cantripSelection", out var spell4)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Druid, 1));
          else ((CantripSelectionWindow)spell4).CreateWindow(ClassType.Druid, 1);

          player.LearnClassSkill(CustomSkill.FormeSauvageRothe);

          break;

        case 5:  player.LearnClassSkill(CustomSkill.DruideReveilSauvage); break;

        case 6:
          player.LearnClassSkill(CustomSkill.FormeSauvagePanthere);
          player.LearnClassSkill(CustomSkill.FormeSauvageOursHibou);
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

          if (!player.windows.TryGetValue("cantripSelection", out var spell10)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Druid, 1));
          else ((CantripSelectionWindow)spell10).CreateWindow(ClassType.Druid, 1);
          
          player.LearnClassSkill(CustomSkill.FormeSauvageDilophosaure);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 18: player.LearnClassSkill(CustomSkill.DruideIncantationBestiale); break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();


          break;

        case 20:
          player.LearnClassSkill(CustomSkill.FormeSauvagePersistante);
          player.LearnClassSkill(CustomSkill.MageNature);
          break;
      }
    }
  }
}
