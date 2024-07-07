using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleGuerreLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 1: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Guerre");
          player.oid.SetTextureOverride("clerc", "guerre");

          foreach (Learnable mastery in Fighter.startingPackage.learnables)
          {
            player.learnableSkills.TryAdd(mastery.id, new LearnableSkill((LearnableSkill)mastery, player));
            player.learnableSkills[mastery.id].source.Add(Category.Class);

            mastery.acquiredPoints += (mastery.pointsToNextLevel - mastery.acquiredPoints) / 4;
          }

          player.learnableSkills.TryAdd(CustomSkill.ClercMartial, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercMartial], player));
          player.learnableSkills[CustomSkill.ClercMartial].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercMartial].source.Add(Category.Class);

          if (player.learnableSpells.TryGetValue((int)Spell.ShieldOfFaith, out var learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.clericDomain = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.ShieldOfFaith], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell = NwSpell.FromSpellType(Spell.ShieldOfFaith);
          int spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          if (player.learnableSpells.TryGetValue((int)Spell.DivineFavor, out learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.clericDomain = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.DivineFavor], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell = NwSpell.FromSpellType(Spell.DivineFavor);
          spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercFrappeGuidee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercFrappeGuidee], player));
          player.learnableSkills[CustomSkill.ClercFrappeGuidee].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercFrappeGuidee].source.Add(Category.Class);

          break;

        case 3:

          if (player.learnableSpells.TryGetValue((int)Spell.MagicWeapon, out var learnable3))
          {
            learnable3.learntFromClasses.Add(CustomClass.Clerc);
            learnable3.clericDomain = true;

            if (learnable3.currentLevel < 1)
              learnable3.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.MagicWeapon], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell3 = NwSpell.FromSpellType(Spell.MagicWeapon);
          int spellLevel3 = spell3.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel3].Add(spell3);

          if (player.learnableSpells.TryGetValue((int)Spell.ShelgarnsPersistentBlade, out learnable3))
          {
            learnable3.learntFromClasses.Add(CustomClass.Clerc);
            learnable3.clericDomain = true;

            if (learnable3.currentLevel < 1)
              learnable3.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.ShelgarnsPersistentBlade], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell3 = NwSpell.FromSpellType(Spell.ShelgarnsPersistentBlade);
          spellLevel3 = spell3.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel3].Add(spell3);

          break;

        case 5:

          if (player.learnableSpells.TryGetValue(CustomSpell.EspritsGardiens, out var learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Clerc);
            learnable5.clericDomain = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.EspritsGardiens], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell5 = NwSpell.FromSpellId(CustomSpell.EspritsGardiens);
          int spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel5].Add(spell5);

          if (player.learnableSpells.TryGetValue(CustomSpell.CapeDuCroise, out learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Clerc);
            learnable5.clericDomain = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.CapeDuCroise], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell5 = NwSpell.FromSpellId(CustomSpell.CapeDuCroise);
          spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel5].Add(spell5);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercLinceulDombre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercLinceulDombre], player));
          player.learnableSkills[CustomSkill.ClercLinceulDombre].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercLinceulDombre].source.Add(Category.Class);

          break;

        case 7:

          if (player.learnableSpells.TryGetValue((int)Spell.Stoneskin, out var learnable7))
          {
            learnable7.learntFromClasses.Add(CustomClass.Clerc);
            learnable7.clericDomain = true;

            if (learnable7.currentLevel < 1)
              learnable7.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Stoneskin], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell7 = NwSpell.FromSpellType(Spell.Stoneskin);
          int spellLevel7 = spell7.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel7].Add(spell7);

          if (player.learnableSpells.TryGetValue((int)Spell.FreedomOfMovement, out learnable7))
          {
            learnable7.learntFromClasses.Add(CustomClass.Clerc);
            learnable7.clericDomain = true;

            if (learnable7.currentLevel < 1)
              learnable7.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.FreedomOfMovement], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell7 = NwSpell.FromSpellType(Spell.FreedomOfMovement);
          spellLevel7 = spell7.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel7].Add(spell7);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercGuerreFrappeDivine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercGuerreFrappeDivine], player));
          player.learnableSkills[CustomSkill.ClercGuerreFrappeDivine].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercGuerreFrappeDivine].source.Add(Category.Class);

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

          if (player.learnableSpells.TryGetValue((int)Spell.HoldMonster, out learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Clerc);
            learnable9.clericDomain = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.HoldMonster], CustomClass.Clerc) { clericDomain = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell9 = NwSpell.FromSpellType(Spell.HoldMonster);
          spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel9].Add(spell9);

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.ClercGuerreAvatarDeBataille, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercGuerreAvatarDeBataille], player));
          player.learnableSkills[CustomSkill.ClercGuerreAvatarDeBataille].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercGuerreAvatarDeBataille].source.Add(Category.Class);

          break;
      }
    }
  }
}
