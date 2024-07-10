using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleNatureLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 1: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Nature");
          player.oid.SetTextureOverride("clerc", "nature_domain");

          if (!player.windows.TryGetValue("spellSelection", out var cantrip1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Druid, 1, 0));
          else ((SpellSelectionWindow)cantrip1).CreateWindow(ClassType.Druid, 1, 0);

          List<int> skillList = new() { CustomSkill.AnimalHandlingProficiency, CustomSkill.NatureProficiency, CustomSkill.SurvivalProficiency };

          if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 1));
          else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 1);

          if (player.learnableSpells.TryGetValue(CustomSpell.SpeakAnimal, out var learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.clericDomain = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.SpeakAnimal], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell = NwSpell.FromSpellId(CustomSpell.SpeakAnimal);
          int spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          if (player.learnableSpells.TryGetValue(CustomSpell.AmitieAnimale, out learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.clericDomain = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.AmitieAnimale], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell = NwSpell.FromSpellId(CustomSpell.AmitieAnimale);
          spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercCharmePlanteEtAnimaux, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercCharmePlanteEtAnimaux], player));
          player.learnableSkills[CustomSkill.ClercCharmePlanteEtAnimaux].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercCharmePlanteEtAnimaux].source.Add(Category.Class);

          break;

        case 3:

          if (player.learnableSpells.TryGetValue(CustomSpell.CroissanceDepines, out var learnable3))
          {
            learnable3.learntFromClasses.Add(CustomClass.Clerc);
            learnable3.clericDomain = true;

            if (learnable3.currentLevel < 1)
              learnable3.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.CroissanceDepines], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell3 = NwSpell.FromSpellId(CustomSpell.CroissanceDepines);
          int spellLevel3 = spell3.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel3].Add(spell3);

          if (player.learnableSpells.TryGetValue((int)Spell.Barkskin, out learnable3))
          {
            learnable3.learntFromClasses.Add(CustomClass.Clerc);
            learnable3.clericDomain = true;

            if (learnable3.currentLevel < 1)
              learnable3.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Barkskin], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell3 = NwSpell.FromSpellType(Spell.Barkskin);
          spellLevel3 = spell3.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel3].Add(spell3);

          break;

        case 5:

          if (player.learnableSpells.TryGetValue(CustomSpell.TempeteDeNeige, out var learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Clerc);
            learnable5.clericDomain = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.TempeteDeNeige], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell5 = NwSpell.FromSpellId(CustomSpell.TempeteDeNeige);
          int spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel5].Add(spell5);

          if (player.learnableSpells.TryGetValue(CustomSpell.CroissanceVegetale, out learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Clerc);
            learnable5.clericDomain = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.CroissanceVegetale], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell5 = NwSpell.FromSpellId(CustomSpell.CroissanceVegetale);
          spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel5].Add(spell5);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercAttenuationElementaire, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercAttenuationElementaire], player));
          player.learnableSkills[CustomSkill.ClercAttenuationElementaire].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercAttenuationElementaire].source.Add(Category.Class);

          break;

        case 7:

          if (player.learnableSpells.TryGetValue(CustomSpell.LianeAvide, out var learnable7))
          {
            learnable7.learntFromClasses.Add(CustomClass.Clerc);
            learnable7.clericDomain = true;

            if (learnable7.currentLevel < 1)
              learnable7.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.LianeAvide], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell7 = NwSpell.FromSpellId(CustomSpell.LianeAvide);
          int spellLevel7 = spell7.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel7].Add(spell7);

          if (player.learnableSpells.TryGetValue((int)Spell.DominateAnimal, out learnable7))
          {
            learnable7.learntFromClasses.Add(CustomClass.Clerc);
            learnable7.clericDomain = true;

            if (learnable7.currentLevel < 1)
              learnable7.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.DominateAnimal], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell7 = NwSpell.FromSpellType(Spell.DominateAnimal);
          spellLevel7 = spell7.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel7].Add(spell7);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercFurieElementaire, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercFurieElementaire], player));
          player.learnableSkills[CustomSkill.ClercFurieElementaire].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercFurieElementaire].source.Add(Category.Class);

          break;

        case 9:

          if (player.learnableSpells.TryGetValue(CustomSpell.MurDePierre, out var learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Clerc);
            learnable9.clericDomain = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.MurDePierre], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell9 = NwSpell.FromSpellId(CustomSpell.MurDePierre);
          int spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel9].Add(spell9);

          if (player.learnableSpells.TryGetValue(CustomSpell.FleauDinsectes, out learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Clerc);
            learnable9.clericDomain = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.FleauDinsectes], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell9 = NwSpell.FromSpellId(CustomSpell.FleauDinsectes);
          spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel9].Add(spell9);

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.ClercHaloDeLumiere, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercHaloDeLumiere], player));
          player.learnableSkills[CustomSkill.ClercHaloDeLumiere].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercHaloDeLumiere].source.Add(Category.Class);

          break;
      }
    }
  }
}
