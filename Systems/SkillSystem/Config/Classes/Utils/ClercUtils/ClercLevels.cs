using System.Linq;
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
            if (Utils.In(spell.Id, CustomSpell.Deguisement, (int)Spell.CharmPerson, CustomSpell.PassageSansTrace, CustomSpell.ImageMiroir, (int)Spell.CallLightning, (int)Spell.ElementalShield,
              (int)Spell.Fear, (int)Spell.PolymorphSelf, (int)Spell.DivineFavor, (int)Spell.MagicWeapon, CustomSpell.PorteDimensionnelle, (int)Spell.SeeInvisibility, (int)Spell.Invisibility,
              (int)Spell.HoldMonster, CustomSpell.CapeDuCroise, (int)Spell.Firebrand, CustomSpell.SphereDeFeuMaster, (int)Spell.Fireball, CustomSpell.AlterationMemorielle, CustomSpell.FaerieFire,
              (int)Spell.WallOfFire, CustomSpell.VagueDestructrice, (int)Spell.CharmPersonOrAnimal, CustomSpell.CroissanceDepines, (int)Spell.Barkskin, (int)Spell.BurningHands, CustomSpell.SpeakAnimal,
              CustomSpell.CroissanceVegetale, CustomSpell.TempeteDeNeige, (int)Spell.DominateAnimal, CustomSpell.LianeAvide, CustomSpell.SphereResilienteDotiluke, CustomSpell.Fracassement, (int)Spell.IceStorm,
              CustomSpell.FleauDinsectes, CustomSpell.MurDePierre, (int)Spell.Sleep, (int)Spell.HoldPerson, (int)Spell.Slow, (int)Spell.Confusion, (int)Spell.Balagarnsironhorn, (int)Spell.GustOfWind,
              (int)Spell.DominatePerson, CustomSpell.Telekinesie, (int)Spell.Identify, CustomSpell.Antidetection, CustomSpell.OeilMagique, CustomSpell.NappeDeBrouillard))
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
