﻿using Anvil.API;
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
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Lumière");
          player.oid.SetTextureOverride("clerc", "light_domain");

          player.learnableSkills.TryAdd(CustomSkill.ClercIllumination, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercIllumination], player));
          player.learnableSkills[CustomSkill.ClercIllumination].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercIllumination].source.Add(Category.Class);

          if (player.learnableSpells.TryGetValue((int)Spell.Light, out var learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Clerc);
            learnable.alwaysPrepared = true;

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.Light], CustomClass.Clerc) { alwaysPrepared = true };
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          NwSpell spell = NwSpell.FromSpellType(Spell.Light);
          int spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);
          player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);

          player.LearnAlwaysPreparedSpell((int)Spell.BurningHands, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.FaerieFire, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Firebrand, CustomSkill.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.SeeInvisibility, CustomSkill.Clerc);

          player.learnableSkills.TryAdd(CustomSkill.ClercRadianceDeLaube, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercRadianceDeLaube], player));
          player.learnableSkills[CustomSkill.ClercRadianceDeLaube].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercRadianceDeLaube].source.Add(Category.Class);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell((int)Spell.Fireball, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.LumiereDuJour, CustomClass.Clerc);

          break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.WallOfFire, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.OeilMagique, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.FlameStrike, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.Scrutation, CustomClass.Clerc);

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
