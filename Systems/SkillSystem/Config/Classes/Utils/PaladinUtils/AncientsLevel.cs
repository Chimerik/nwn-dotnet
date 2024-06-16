﻿using System.Security.Cryptography;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static void HandleAnciensLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Serment des Anciens");
          player.oid.SetTextureOverride("paladin", "anciens");

          player.learnableSkills.TryAdd(CustomSkill.AnciensGuerisonRayonnante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AnciensGuerisonRayonnante], player));
          player.learnableSkills[CustomSkill.AnciensGuerisonRayonnante].LevelUp(player);
          player.learnableSkills[CustomSkill.AnciensGuerisonRayonnante].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.AnciensCourrouxDeLaNature, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AnciensCourrouxDeLaNature], player));
          player.learnableSkills[CustomSkill.AnciensCourrouxDeLaNature].LevelUp(player);
          player.learnableSkills[CustomSkill.AnciensCourrouxDeLaNature].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.AnciensRenvoiDesInfideles, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AnciensRenvoiDesInfideles], player));
          player.learnableSkills[CustomSkill.AnciensRenvoiDesInfideles].LevelUp(player);
          player.learnableSkills[CustomSkill.AnciensRenvoiDesInfideles].source.Add(Category.Class);

          if (player.learnableSpells.TryGetValue(CustomSpell.SpeakAnimal, out var learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Paladin);
            learnable.paladinSerment = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.SpeakAnimal], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell = NwSpell.FromSpellId(CustomSpell.SpeakAnimal);
          int spellLevel = spell.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel].Add(spell);

          if (player.learnableSpells.TryGetValue(CustomSpell.FrappePiegeuse, out learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Paladin);
            learnable.paladinSerment= true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.FrappePiegeuse], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell = NwSpell.FromSpellId(CustomSpell.FrappePiegeuse);
          spellLevel = spell.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel].Add(spell);

          break;

        case 5:

          if (player.learnableSpells.TryGetValue((int)Spell.LesserRestoration, out var learnable5))
          {
            learnable5.learntFromClasses.Add(CustomClass.Paladin);
            learnable5.paladinSerment = true;

            if (learnable5.currentLevel < 1)
              learnable5.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.LesserRestoration], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell5 = NwSpell.FromSpellType(Spell.LesserRestoration);
          int spellLevel5 = spell5.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel5].Add(spell5);

          if (player.learnableSpells.TryGetValue((int)Spell.Silence, out learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Paladin);
            learnable.paladinSerment = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Silence], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell = NwSpell.FromSpellType(Spell.Silence);
          spellLevel = spell.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel].Add(spell);

          break;


        case 7:

          player.learnableSkills.TryAdd(CustomSkill.PaladinAuraDeDevotion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinAuraDeDevotion], player));
          player.learnableSkills[CustomSkill.PaladinAuraDeDevotion].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinAuraDeDevotion].source.Add(Category.Class);

          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeGarde(7)));

          break;

        case 9:

          if (player.learnableSpells.TryGetValue((int)Spell.RemoveCurse, out var learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Paladin);
            learnable9.paladinSerment = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.RemoveCurse], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell9 = NwSpell.FromSpellType(Spell.RemoveCurse);
          int spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel9].Add(spell9);

          if (player.learnableSpells.TryGetValue(CustomSpell.LueurDespoir, out learnable9))
          {
            learnable9.learntFromClasses.Add(CustomClass.Paladin);
            learnable9.paladinSerment = true;

            if (learnable9.currentLevel < 1)
              learnable9.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.LueurDespoir], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell9 = NwSpell.FromSpellId(CustomSpell.LueurDespoir);
          spellLevel9 = spell9.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel9].Add(spell9);

          break;

        case 13:

          if (player.learnableSpells.TryGetValue((int)Spell.FreedomOfMovement, out var learnable13))
          {
            learnable13.learntFromClasses.Add(CustomClass.Paladin);
            learnable13.paladinSerment = true;

            if (learnable13.currentLevel < 1)
              learnable13.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.FreedomOfMovement], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell13 = NwSpell.FromSpellType(Spell.FreedomOfMovement);
          int spellLevel13 = spell13.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel13].Add(spell13);

          if (player.learnableSpells.TryGetValue(CustomSpell.GardienDeLaFoi, out learnable13))
          {
            learnable13.learntFromClasses.Add(CustomClass.Paladin);
            learnable13.paladinSerment = true;

            if (learnable13.currentLevel < 1)
              learnable13.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.GardienDeLaFoi], CustomClass.Paladin) { paladinSerment = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          spell13 = NwSpell.FromSpellId(CustomSpell.GardienDeLaFoi);
          spellLevel13 = spell13.GetSpellLevelForClass(ClassType.Paladin);
          player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel13].Add(spell13);

          break;

        case 15:

          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ProtectionContreLeMalEtLeBien));

          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.SentinelleImmortelle);
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).Value = 1;
          player.oid.LoginCreature.OnDamaged += PaladinUtils.HandleSentinelleImmortelle;

          break;

        case 18:

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeGardeEffectTag);
          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeGarde(18)));

          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.DevotionNimbeSacree, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DevotionNimbeSacree], player));
          player.learnableSkills[CustomSkill.DevotionNimbeSacree].LevelUp(player);
          player.learnableSkills[CustomSkill.DevotionNimbeSacree].source.Add(Category.Class);

          break;
      }
    }
  }
}
