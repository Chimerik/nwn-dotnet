﻿using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Occultiste
  {
    public static void HandleFielonLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(player.oid, "Mécène Fiélon");
          player.oid.SetTextureOverride("occultiste", "warlock_fielon");

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.BurningHands, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Firebrand, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Injonction, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Suggestion, CustomClass.Occultiste);
          

          player.learnableSkills.TryAdd(CustomSkill.BenedictionDuMalin, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BenedictionDuMalin], player));
          player.learnableSkills[CustomSkill.BenedictionDuMalin].LevelUp(player);
          player.learnableSkills[CustomSkill.BenedictionDuMalin].source.Add(Category.Class);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.StinkingCloud, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Fireball, CustomClass.Occultiste);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.FaveurDuMalin, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FaveurDuMalin], player));
          player.learnableSkills[CustomSkill.FaveurDuMalin].LevelUp(player);
          player.learnableSkills[CustomSkill.FaveurDuMalin].source.Add(Category.Class);

          break;

        case 7:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.WallOfFire, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ElementalShield, CustomClass.Occultiste);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FleauDinsectes, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Quete, CustomClass.Occultiste);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.ResilienceFielleuse, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ResilienceFielleuse], player));
          player.learnableSkills[CustomSkill.ResilienceFielleuse].LevelUp(player);
          player.learnableSkills[CustomSkill.ResilienceFielleuse].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.TraverseeInfernale, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TraverseeInfernale], player));
          player.learnableSkills[CustomSkill.TraverseeInfernale].LevelUp(player);
          player.learnableSkills[CustomSkill.TraverseeInfernale].source.Add(Category.Class);

          break;
      }
    }
  }
}