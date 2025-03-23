using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static void HandlePaladinLevelUp(Player player, int level, LearnableSkill playerClass)
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

          foreach(var spell in NwRuleset.Spells.Where(s => 0 < s.GetSpellLevelForClass(ClassType.Paladin) && s.GetSpellLevelForClass(ClassType.Paladin) < 10))
          {
            if (Utils.In(spell.Id, CustomSpell.GardienDeLaFoi, CustomSpell.LueurDespoir, CustomSpell.FrappePiegeuse, 
              CustomSpell.FouleeBrumeuse, CustomSpell.RayonDeLuneMaster, (int)Spell.ProtectionFromElements, CustomSpell.CroissanceVegetale,
              (int)Spell.Silence, (int)Spell.Stoneskin, (int)Spell.IceStorm, CustomSpell.CommunionAvecLaNature, CustomSpell.PassageParLesArbres, 
              (int)Spell.FlameStrike, CustomSpell.Communion, (int)Spell.Bane, CustomSpell.MarqueDuChasseur, (int)Spell.HoldPerson, CustomSpell.FouleeBrumeuse,
              (int)Spell.Haste, CustomSpell.Bannissement, CustomSpell.PorteDimensionnelle, (int)Spell.HoldMonster, CustomSpell.Scrutation)) 
                continue;// ces sorts ne font pas partie du package de paladin mais peuvent être appris via le serment
            
            if (player.learnableSpells.TryGetValue(spell.Id, out var learnable))
            {
              learnable.learntFromClasses.Add((int)ClassType.Paladin);

              if (learnable.currentLevel < 1)
                learnable.LevelUp(player);
            }
            else
            {
              LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[spell.Id], (int)ClassType.Paladin);
              player.learnableSpells.Add(learnableSpell.id, learnableSpell);
              learnableSpell.LevelUp(player);
            }
          }

          // On donne les autres capacités de niveau 1

          player.LearnClassSkill(CustomSkill.ImpositionDesMains);

          if (!player.windows.TryGetValue("expertiseDarmeSelection", out var invo1)) player.windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(player, 2));
          else ((ExpertiseDarmeSelectionWindow)invo1).CreateWindow(2);

          break;

        case 2:

          if (!player.windows.TryGetValue("fightingStyleSelection", out var style)) player.windows.Add("fightingStyleSelection", new FightingStyleSelectionWindow(player, CustomSkill.Paladin));
          else ((FightingStyleSelectionWindow)style).CreateWindow(CustomSkill.Paladin);

          player.LearnClassSkill(CustomSkill.ChatimentDivin);
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentDivin, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(1));

          break;

        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value = CustomSkill.Paladin;

          if (!player.windows.TryGetValue("subClassSelection", out var value)) player.windows.Add("subClassSelection", new SubClassSelectionWindow(player));
          else ((SubClassSelectionWindow)value).CreateWindow();

          player.LearnClassSkill(CustomSkill.SensDivin);

          break;

        case 4:

          if (!player.windows.TryGetValue("featSelection", out var feat4)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat4).CreateWindow();

          break;

        case 5:

          player.LearnClassSkill(CustomSkill.AttaqueSupplementaire);
          CreatureUtils.InitializeNumAttackPerRound(player.oid.LoginCreature);
            
            break;

        case 6: player.LearnClassSkill(CustomSkill.AuraDeProtection); break;

        case 8:

          if (!player.windows.TryGetValue("featSelection", out var feat8)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat8).CreateWindow();

          break;

        case 9: player.LearnClassSkill(CustomSkill.PaladinConspuerEnnemi); break;

        case 10:

          var auraDeProtection = NwFeat.FromFeatId(CustomSkill.AuraDeProtection);
          player.LearnClassSkill(CustomSkill.AuraDeCourage);

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeProtectionEffectTag);

          if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PaladinSermentVengeance))
          {
            var nameTlk = auraDeProtection.Name;
            nameTlk.SetPlayerOverride(player.oid, "Aura de Courage");
            player.oid.SetTextureOverride(auraDeProtection.IconResRef, NwFeat.FromFeatId(CustomSkill.AuraDeCourage).IconResRef);
          }

          var descTlk = auraDeProtection.Description;
          descTlk.SetPlayerOverride(player.oid, $"{descTlk.ToString()}\n\n{player.learnableSkills[CustomSkill.AuraDeCourage].description}");

          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(player.oid.LoginCreature, 10));
          UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(3);

          break;

        case 11: player.LearnClassSkill(CustomSkill.PaladinChatimentAmeliore); break;

        case 12:

          if (!player.windows.TryGetValue("featSelection", out var feat12)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat12).CreateWindow();

          break;

        case 16:

          if (!player.windows.TryGetValue("featSelection", out var feat16)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat16).CreateWindow();

          break;

        case 18:

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeProtectionEffectTag);
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(player.oid.LoginCreature, 18));
          UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(9);

          break;

        case 19:

          if (!player.windows.TryGetValue("featSelection", out var feat19)) player.windows.Add("featSelection", new FeatSelectionWindow(player));
          else ((FeatSelectionWindow)feat19).CreateWindow();

          break;
      }
    }
  }
}
