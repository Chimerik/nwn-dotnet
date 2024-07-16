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

          ClercUtils.LearnDomaineSpell(player, (int)Spell.BurningHands);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.FaerieFire);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercRadianceDeLaube, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercRadianceDeLaube], player));
          player.learnableSkills[CustomSkill.ClercRadianceDeLaube].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercRadianceDeLaube].source.Add(Category.Class);

          break;

        case 3:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.Firebrand);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.SphereDeFeu);

          break;

        case 5:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.Fireball);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.LumiereDuJour);

          break;

        case 7:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.WallOfFire);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.GardienDeLaFoi);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercIncantationPuissante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercIncantationPuissante], player));
          player.learnableSkills[CustomSkill.ClercIncantationPuissante].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercIncantationPuissante].source.Add(Category.Class);

          break;

        case 9:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.FlameStrike);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.VagueDestructrice);

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
