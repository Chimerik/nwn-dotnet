using System.Security.Cryptography;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static void HandleVengeanceLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Serment de Vengeance");
          player.oid.SetTextureOverride("paladin", "vengeance");

          player.learnableSkills.TryAdd(CustomSkill.PaladinVoeuHostile, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinVoeuHostile], player));
          player.learnableSkills[CustomSkill.PaladinVoeuHostile].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinVoeuHostile].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.PaladinPuissanceInquisitrice, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinPuissanceInquisitrice], player));
          player.learnableSkills[CustomSkill.PaladinPuissanceInquisitrice].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinPuissanceInquisitrice].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.PaladinConspuerEnnemi, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinConspuerEnnemi], player));
          player.learnableSkills[CustomSkill.PaladinConspuerEnnemi].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinConspuerEnnemi].source.Add(Category.Class);

          if (player.learnableSpells.TryGetValue(CustomSpell.MarqueDuChasseur, out var learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Paladin);
            learnable.paladinSerment = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.MarqueDuChasseur], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell = NwSpell.FromSpellId(CustomSpell.MarqueDuChasseur);
          int spellLevel = spell.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel].Add(spell);

          if (player.learnableSpells.TryGetValue((int)Spell.Bane, out learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Paladin);
            learnable.paladinSerment= true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Bane], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell = NwSpell.FromSpellType(Spell.Bane);
          spellLevel = spell.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel].Add(spell);

          break;

        case 5:

          if (player.learnableSpells.TryGetValue(CustomSpell.FouleeBrumeuse, out var learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Paladin);
            learnable5.paladinSerment = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.FouleeBrumeuse], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell5 = NwSpell.FromSpellId(CustomSpell.FouleeBrumeuse);
          int spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel5].Add(spell5);

          if (player.learnableSpells.TryGetValue((int)Spell.HoldPerson, out learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Paladin);
            learnable.paladinSerment = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.HoldPerson], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell = NwSpell.FromSpellType(Spell.HoldPerson);
          spellLevel = spell.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel].Add(spell);

          break;


        case 7:

          player.learnableSkills.TryAdd(CustomSkill.PaladinVengeurImplacable, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinVengeurImplacable], player));
          player.learnableSkills[CustomSkill.PaladinVengeurImplacable].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinVengeurImplacable].source.Add(Category.Class);

          player.oid.LoginCreature.OnCreatureAttack -= PaladinUtils.OnAttackVengeurImplacable;
          player.oid.LoginCreature.OnCreatureAttack += PaladinUtils.OnAttackVengeurImplacable;

          break;

        case 9:

          if (player.learnableSpells.TryGetValue((int)Spell.ProtectionFromElements, out var learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Paladin);
            learnable9.paladinSerment = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.ProtectionFromElements], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell9 = NwSpell.FromSpellType(Spell.ProtectionFromElements);
          int spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel9].Add(spell9);

          if (player.learnableSpells.TryGetValue((int)Spell.Haste, out learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Paladin);
            learnable9.paladinSerment = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Haste], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell9 = NwSpell.FromSpellType(Spell.Haste);
          spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel9].Add(spell9);

          break;

        case 13:

          if (player.learnableSpells.TryGetValue(CustomSpell.Bannissement, out var learnable13))
          {
            learnable13.learntFromClasses.Add(CustomClass.Paladin);
            learnable13.paladinSerment = true;

            if (learnable13.currentLevel < 1)
              learnable13.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.Bannissement], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell13 = NwSpell.FromSpellId(CustomSpell.Bannissement);
          int spellLevel13 = spell13.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel13].Add(spell13);

          if (player.learnableSpells.TryGetValue(CustomSpell.PorteDimensionnelle, out learnable13))
          {
            learnable13.learntFromClasses.Add(CustomClass.Paladin);
            learnable13.paladinSerment = true;

            if (learnable13.currentLevel < 1)
              learnable13.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.PorteDimensionnelle], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell13 = NwSpell.FromSpellId(CustomSpell.PorteDimensionnelle);
          spellLevel13 = spell13.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel13].Add(spell13);

          break;

        case 17:

          if (player.learnableSpells.TryGetValue((int)Spell.HoldMonster, out var learnable17))
          {
            learnable17.learntFromClasses.Add(CustomClass.Paladin);
            learnable17.paladinSerment = true;

            if (learnable17.currentLevel < 1)
              learnable17.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.HoldMonster], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell17 = NwSpell.FromSpellType(Spell.HoldMonster);
          int spellLevel17 = spell17.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel17].Add(spell17);

          if (player.learnableSpells.TryGetValue(CustomSpell.Scrutation, out learnable17))
          {
            learnable17.learntFromClasses.Add(CustomClass.Paladin);
            learnable17.paladinSerment = true;

            if (learnable17.currentLevel < 1)
              learnable17.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.Scrutation], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell17 = NwSpell.FromSpellId(CustomSpell.Scrutation);
          spellLevel17 = spell17.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel17].Add(spell17);

          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.AngeDeLaVengeance, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AngeDeLaVengeance], player));
          player.learnableSkills[CustomSkill.AngeDeLaVengeance].LevelUp(player);
          player.learnableSkills[CustomSkill.AngeDeLaVengeance].source.Add(Category.Class);

          break;
      }
    }
  }
}
