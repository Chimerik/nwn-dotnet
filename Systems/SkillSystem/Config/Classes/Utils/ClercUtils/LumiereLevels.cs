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

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.BurningHands, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FaerieFire, CustomClass.Clerc);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercRadianceDeLaube, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercRadianceDeLaube], player));
          player.learnableSkills[CustomSkill.ClercRadianceDeLaube].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercRadianceDeLaube].source.Add(Category.Class);

          break;

        case 3:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Firebrand, CustomSkill.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.SphereDeFeu, CustomClass.Clerc);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Fireball, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.LumiereDuJour, CustomClass.Clerc);

          break;

        case 7:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.WallOfFire, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.GardienDeLaFoi, CustomClass.Clerc);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercIncantationPuissante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercIncantationPuissante], player));
          player.learnableSkills[CustomSkill.ClercIncantationPuissante].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercIncantationPuissante].source.Add(Category.Class);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.FlameStrike, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.VagueDestructrice, CustomClass.Clerc);

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
