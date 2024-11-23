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

          player.LearnAlwaysPreparedSpell((int)Spell.CureModerateWounds, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell(CustomSpell.RayonDeLune, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell(CustomSpell.LueurEtoilee, CustomClass.Druid);
          
          break;

        case 4:

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageCorbeau, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageCorbeau], player));
          player.learnableSkills[CustomSkill.FormeSauvageCorbeau].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageCorbeau].source.Add(Category.Class);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell((int)Spell.SummonCreatureIii, CustomClass.Druid);

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

          player.LearnAlwaysPreparedSpell(CustomSpell.PuitsDeLune, CustomClass.Druid);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageTigre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageTigre], player));
          player.learnableSkills[CustomSkill.FormeSauvageTigre].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageTigre].source.Add(Category.Class);

          break;

        case 9:

            player.LearnAlwaysPreparedSpell((int)Spell.MassHeal, CustomClass.Druid);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.DruideProtectionNaturelle, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DruideProtectionNaturelle], player));
          player.learnableSkills[CustomSkill.DruideProtectionNaturelle].LevelUp(player);
          player.learnableSkills[CustomSkill.DruideProtectionNaturelle].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageAir, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageAir], player));
          player.learnableSkills[CustomSkill.FormeSauvageAir].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageAir].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageTerre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageTerre], player));
          player.learnableSkills[CustomSkill.FormeSauvageTerre].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageTerre].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageFeu, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageFeu], player));
          player.learnableSkills[CustomSkill.FormeSauvageFeu].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageFeu].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FormeSauvageEau, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FormeSauvageEau], player));
          player.learnableSkills[CustomSkill.FormeSauvageEau].LevelUp(player);
          player.learnableSkills[CustomSkill.FormeSauvageEau].source.Add(Category.Class);

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
