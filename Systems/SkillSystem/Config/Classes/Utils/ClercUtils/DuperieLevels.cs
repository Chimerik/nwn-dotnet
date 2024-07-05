using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleDuperieLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 1: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Duperie");
          player.oid.SetTextureOverride("clerc", "duperie");

          player.learnableSkills.TryAdd(CustomSkill.ClercBenedictionEscroc, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercBenedictionEscroc], player));
          player.learnableSkills[CustomSkill.ClercBenedictionEscroc].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercBenedictionEscroc].source.Add(Category.Class);

          if (player.learnableSpells.TryGetValue(CustomSpell.Deguisement, out var learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.clericDomain = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.Deguisement], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell = NwSpell.FromSpellId(CustomSpell.Deguisement);
          int spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          if (player.learnableSpells.TryGetValue((int)Spell.CharmPerson, out learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.clericDomain = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.CharmPerson], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell = NwSpell.FromSpellType(Spell.CharmPerson);
          spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercRepliqueInvoquee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercRepliqueInvoquee], player));
          player.learnableSkills[CustomSkill.ClercRepliqueInvoquee].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercRepliqueInvoquee].source.Add(Category.Class);

          break;

        case 3:

          if (player.learnableSpells.TryGetValue(CustomSpell.PassageSansTrace, out var learnable3))
          {
            learnable3.learntFromClasses.Add(CustomClass.Clerc);
            learnable3.clericDomain = true;

            if (learnable3.currentLevel < 1)
              learnable3.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.PassageSansTrace], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell3 = NwSpell.FromSpellId(CustomSpell.PassageSansTrace);
          int spellLevel3 = spell3.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel3].Add(spell3);

          if (player.learnableSpells.TryGetValue(CustomSpell.ImageMiroir, out learnable3))
          {
            learnable3.learntFromClasses.Add(CustomClass.Clerc);
            learnable3.clericDomain = true;

            if (learnable3.currentLevel < 1)
              learnable3.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.ImageMiroir], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell3 = NwSpell.FromSpellId(CustomSpell.ImageMiroir);
          spellLevel3 = spell3.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel3].Add(spell3);

          break;

        case 5:

          if (player.learnableSpells.TryGetValue((int)Spell.Fear, out var learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Clerc);
            learnable5.clericDomain = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Fear], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell5 = NwSpell.FromSpellType(Spell.Fear);
          int spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel5].Add(spell5);

          if (player.learnableSpells.TryGetValue((int)Spell.BestowCurse, out learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Clerc);
            learnable5.clericDomain = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.BestowCurse], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell5 = NwSpell.FromSpellType(Spell.BestowCurse);
          spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel5].Add(spell5);

          break;


        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercLinceulDombre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercLinceulDombre], player));
          player.learnableSkills[CustomSkill.ClercLinceulDombre].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercLinceulDombre].source.Add(Category.Class);

          break;

        case 7:

          if (player.learnableSpells.TryGetValue((int)Spell.PolymorphSelf, out var learnable7))
          {
            learnable7.learntFromClasses.Add(CustomClass.Clerc);
            learnable7.clericDomain = true;

            if (learnable7.currentLevel < 1)
              learnable7.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.PolymorphSelf], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell7 = NwSpell.FromSpellType(Spell.PolymorphSelf);
          int spellLevel7 = spell7.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel7].Add(spell7);

          if (player.learnableSpells.TryGetValue(CustomSpell.PorteDimensionnelle, out learnable7))
          {
            learnable7.learntFromClasses.Add(CustomClass.Clerc);
            learnable7.clericDomain = true;

            if (learnable7.currentLevel < 1)
              learnable7.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.PorteDimensionnelle], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell7 = NwSpell.FromSpellId(CustomSpell.PorteDimensionnelle);
          spellLevel7 = spell7.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel7].Add(spell7);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercFrappeDivine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercFrappeDivine], player));
          player.learnableSkills[CustomSkill.ClercFrappeDivine].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercFrappeDivine].source.Add(Category.Class);

          break;

        case 9:

          if (player.learnableSpells.TryGetValue((int)Spell.DominatePerson, out var learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Clerc);
            learnable9.clericDomain = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.DominatePerson], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell9 = NwSpell.FromSpellType(Spell.DominatePerson);
          int spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel9].Add(spell9);

          if (player.learnableSpells.TryGetValue(CustomSpell.ApparencesTrompeuses, out learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Clerc);
            learnable9.clericDomain = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.ApparencesTrompeuses], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell9 = NwSpell.FromSpellId(CustomSpell.ApparencesTrompeuses);
          spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel9].Add(spell9);

          break;
      }
    }
  }
}
