﻿using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleClercLevelUp(Player player, int level, LearnableSkill playerClass)
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

          foreach(var spell in NwRuleset.Spells.Where(s => s.GetSpellLevelForClass(ClassType.Cleric) > 0 && s.GetSpellLevelForClass(ClassType.Cleric) < 10))
          {
            if (Utils.In(spell.Id, CustomSpell.Deguisement, (int)Spell.CharmPerson, CustomSpell.PassageSansTrace, CustomSpell.ImageMiroir,
              (int)Spell.Fear, (int)Spell.PolymorphSelf, (int)Spell.DivineFavor, (int)Spell.MagicWeapon, CustomSpell.PorteDimensionnelle,
              (int)Spell.HoldMonster, CustomSpell.CapeDuCroise, (int)Spell.Firebrand, CustomSpell.SphereDeFeu, (int)Spell.Fireball,
              (int)Spell.WallOfFire, CustomSpell.VagueDestructrice, CustomSpell.AmitieAnimale, CustomSpell.CroissanceDepines, (int)Spell.Barkskin,
              CustomSpell.CroissanceVegetale, CustomSpell.TempeteDeNeige, (int)Spell.DominateAnimal, CustomSpell.LianeAvide, CustomSpell.SphereResilienteDotiluke,
              CustomSpell.FleauDinsectes, CustomSpell.MurDePierre, (int)Spell.Sleep, (int)Spell.HoldPerson, (int)Spell.Slow, (int)Spell.Confusion,
              (int)Spell.DominatePerson, CustomSpell.Telekinesie, (int)Spell.Identify, CustomSpell.Antidetection, CustomSpell.OeilMagique,
              CustomSpell.NappeDeBrouillard, (int)Spell.Balagarnsironhorn, (int)Spell.GustOfWind, CustomSpell.Fracassement, (int)Spell.IceStorm,
              (int)Spell.CallLightning, (int)Spell.SeeInvisibility, (int)Spell.Invisibility, CustomSpell.AlterationMemorielle))
              continue;// ces sorts ne font pas partie du package de clerc mais peuvent être appris via le domaine

            if (player.learnableSpells.TryGetValue(spell.Id, out var learnable))
            {
              learnable.learntFromClasses.Add((int)ClassType.Cleric);

              if (learnable.currentLevel < 1)
                learnable.LevelUp(player);
            }
            else
            {
              LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[spell.Id], (int)ClassType.Cleric);
              player.learnableSpells.Add(learnableSpell.id, learnableSpell);
              learnableSpell.LevelUp(player);
            }
          }

          // On donne les autres capacités de niveau 1

          if (!player.windows.TryGetValue("ordreDivinSelection", out var ordreDivin)) player.windows.Add("ordreDivinSelection", new OrdreDivinSelectionWindow(player));
          else ((OrdreDivinSelectionWindow)ordreDivin).CreateWindow();

          if (!player.windows.TryGetValue("cantripSelection", out var cantrip1)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Cleric, 3));
          else ((CantripSelectionWindow)cantrip1).CreateWindow(ClassType.Cleric, 3);

          break;

        case 2:

          player.LearnClassSkill(CustomSkill.ClercRenvoiDesMortsVivants);
          player.LearnClassSkill(CustomSkill.ClercEtincelleDivine);

          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Clerc;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          if(!player.windows.TryGetValue("cantripSelection", out var cantrip4)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Cleric, 1));
          else ((CantripSelectionWindow)cantrip4).CreateWindow(ClassType.Cleric, 1);

          break;

        case 7:

          if (!player.windows.TryGetValue("frappesBeniesSelection", out var frappes)) player.windows.Add("frappesBeniesSelection", new FrappesBeniesSelectionWindow(player));
          else ((FrappesBeniesSelectionWindow)frappes).CreateWindow();

          break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 10:

          if (!player.windows.TryGetValue("cantripSelection", out var cantrip10)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Cleric, 1));
          else ((CantripSelectionWindow)cantrip10).CreateWindow(ClassType.Cleric, 1);

          player.LearnClassSkill(CustomSkill.ClercInterventionDivine);

          break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          break;
      }
      
      ClercUtils.RestoreConduitDivin(player.oid.LoginCreature);
    }
  }
}
