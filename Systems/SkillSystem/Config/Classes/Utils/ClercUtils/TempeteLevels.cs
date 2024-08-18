using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleTempeteLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 1: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Tempête");
          player.oid.SetTextureOverride("clerc", "domaine_tempete");

          foreach (Learnable mastery in Fighter.startingPackage.learnables)
          {
            player.learnableSkills.TryAdd(mastery.id, new LearnableSkill((LearnableSkill)mastery, player));
            player.learnableSkills[mastery.id].source.Add(Category.Class);

            mastery.acquiredPoints += (mastery.pointsToNextLevel - mastery.acquiredPoints) / 4;
          }

          player.learnableSkills[CustomSkill.HeavyArmorProficiency].acquiredPoints = 0;

          player.learnableSkills.TryAdd(CustomSkill.ClercFureurOuraganFoudre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercFureurOuraganFoudre], player));
          player.learnableSkills[CustomSkill.ClercFureurOuraganFoudre].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercFureurOuraganFoudre].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.ClercFureurOuraganTonnerre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercFureurOuraganTonnerre], player));
          player.learnableSkills[CustomSkill.ClercFureurOuraganTonnerre].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercFureurOuraganTonnerre].source.Add(Category.Class);

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.NappeDeBrouillard, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Balagarnsironhorn, CustomClass.Clerc);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercFureurDestructrice, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercFureurDestructrice], player));
          player.learnableSkills[CustomSkill.ClercFureurDestructrice].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercFureurDestructrice].source.Add(Category.Class);

          break;

        case 3:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Fracassement, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.GustOfWind, CustomClass.Clerc);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.TempeteDeNeige, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.CallLightning, CustomClass.Clerc);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercElectrocution, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercElectrocution], player));
          player.learnableSkills[CustomSkill.ClercElectrocution].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercElectrocution].source.Add(Category.Class);

          break;

        case 7:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.ControleDeLeau, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.IceStorm, CustomClass.Clerc);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercTempeteFrappeDivine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercTempeteFrappeDivine], player));
          player.learnableSkills[CustomSkill.ClercTempeteFrappeDivine].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercTempeteFrappeDivine].source.Add(Category.Class);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FleauDinsectes, CustomClass.Clerc);
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
