using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Druide
  {
    public static void HandleCercleSeleniteLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(3).SetPlayerOverride(player.oid, "Cercle Sélénite");
          player.oid.SetTextureOverride("druide", "druide_lune");

          player.learnableSkills.TryAdd(CustomSkill.DruideFormeDeLune, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DruideFormeDeLune], player));
          player.learnableSkills[CustomSkill.DruideFormeDeLune].LevelUp(player);
          player.learnableSkills[CustomSkill.DruideFormeDeLune].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageOurs, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageOurs], player));
          player.learnableSkills[CustomSkill.FormeSauvageOurs].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageOurs].source.Add(Category.Class);

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.CureModerateWounds, CustomClass.Druid);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.RayonDeLune, CustomClass.Druid);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.LueurEtoilee, CustomClass.Druid);
          
          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.SummonCreatureIii, CustomClass.Druid);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.DruideResilienceSauvage, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DruideResilienceSauvage], player));
          player.learnableSkills[CustomSkill.DruideResilienceSauvage].LevelUp(player);
          player.learnableSkills[CustomSkill.DruideResilienceSauvage].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DruideLuneRadieuse, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DruideLuneRadieuse], player));
          player.learnableSkills[CustomSkill.DruideLuneRadieuse].LevelUp(player);
          player.learnableSkills[CustomSkill.DruideLuneRadieuse].source.Add(Category.Class);

          break;

        case 7:
          
          if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreAride))
            SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Enervation, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerrePolaire))
            SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.IceStorm, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTempere))
            SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.FreedomOfMovement, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTropicale))
            SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.PolymorphSelf, CustomClass.Druid);
          
          break;

        case 9:

            SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.MassHeal, CustomClass.Druid);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.DruideProtectionNaturelle, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DruideProtectionNaturelle], player));
          player.learnableSkills[CustomSkill.DruideProtectionNaturelle].LevelUp(player);
          player.learnableSkills[CustomSkill.DruideProtectionNaturelle].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.DruideSanctuaireNaturel, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DruideSanctuaireNaturel], player));
          player.learnableSkills[CustomSkill.DruideSanctuaireNaturel].LevelUp(player);
          player.learnableSkills[CustomSkill.DruideSanctuaireNaturel].source.Add(Category.Class);

          break;
      }
    }
  }
}
