using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleLumiereLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 1: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Lumière");
          player.oid.SetTextureOverride("clerc", "light_domain");

          player.learnableSkills.TryAdd(CustomSkill.ClercIllumination, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercIllumination], player));
          player.learnableSkills[CustomSkill.ClercIllumination].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercIllumination].source.Add(Category.Class);

          if (player.learnableSpells.TryGetValue((int)Spell.Light, out var learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.clericDomain = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Light], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell = NwSpell.FromSpellType(Spell.Light);
          int spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          if (player.learnableSpells.TryGetValue((int)Spell.BurningHands, out learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.clericDomain = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.BurningHands], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell = NwSpell.FromSpellType(Spell.BurningHands);
          spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          if (player.learnableSpells.TryGetValue(CustomSpell.FaerieFire, out learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.clericDomain = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.FaerieFire], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell = NwSpell.FromSpellId(CustomSpell.FaerieFire);
          spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercRadianceDeLaube, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercRadianceDeLaube], player));
          player.learnableSkills[CustomSkill.ClercRadianceDeLaube].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercRadianceDeLaube].source.Add(Category.Class);

          break;

        case 3:

          if (player.learnableSpells.TryGetValue((int)Spell.Firebrand, out var learnable3))
          {
            learnable3.learntFromClasses.Add(CustomClass.Clerc);
            learnable3.clericDomain = true;

            if (learnable3.currentLevel < 1)
              learnable3.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Firebrand], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell3 = NwSpell.FromSpellType(Spell.Firebrand);
          int spellLevel3 = spell3.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel3].Add(spell3);

          if (player.learnableSpells.TryGetValue(CustomSpell.SphereDeFeu, out learnable3))
          {
            learnable3.learntFromClasses.Add(CustomClass.Clerc);
            learnable3.clericDomain = true;

            if (learnable3.currentLevel < 1)
              learnable3.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.SphereDeFeu], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell3 = NwSpell.FromSpellId(CustomSpell.SphereDeFeu);
          spellLevel3 = spell3.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel3].Add(spell3);

          break;

        case 5:

          if (player.learnableSpells.TryGetValue((int)Spell.Fireball, out var learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Clerc);
            learnable5.clericDomain = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Fireball], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell5 = NwSpell.FromSpellType(Spell.Fireball);
          int spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel5].Add(spell5);

          if (player.learnableSpells.TryGetValue(CustomSpell.LumiereDuJour, out learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Clerc);
            learnable5.clericDomain = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.LumiereDuJour], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell5 = NwSpell.FromSpellId(CustomSpell.LumiereDuJour);
          spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel5].Add(spell5);

          break;

        case 7:

          if (player.learnableSpells.TryGetValue(CustomSpell.GardienDeLaFoi, out var learnable7))
          {
            learnable7.learntFromClasses.Add(CustomClass.Clerc);
            learnable7.clericDomain = true;

            if (learnable7.currentLevel < 1)
              learnable7.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.GardienDeLaFoi], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell7 = NwSpell.FromSpellId(CustomSpell.GardienDeLaFoi);
          int spellLevel7 = spell7.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel7].Add(spell7);

          if (player.learnableSpells.TryGetValue((int)Spell.WallOfFire, out learnable7))
          {
            learnable7.learntFromClasses.Add(CustomClass.Clerc);
            learnable7.clericDomain = true;

            if (learnable7.currentLevel < 1)
              learnable7.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.WallOfFire], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell7 = NwSpell.FromSpellType(Spell.WallOfFire);
          spellLevel7 = spell7.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel7].Add(spell7);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercIncantationPuissante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercIncantationPuissante], player));
          player.learnableSkills[CustomSkill.ClercIncantationPuissante].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercIncantationPuissante].source.Add(Category.Class);

          break;

        case 9:

          if (player.learnableSpells.TryGetValue((int)Spell.FlameStrike, out var learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Clerc);
            learnable9.clericDomain = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.FlameStrike], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell9 = NwSpell.FromSpellType(Spell.FlameStrike);
          int spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel9].Add(spell9);

          if (player.learnableSpells.TryGetValue(CustomSpell.VagueDestructrice, out learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Clerc);
            learnable9.clericDomain = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.VagueDestructrice], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell9 = NwSpell.FromSpellId(CustomSpell.VagueDestructrice);
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
