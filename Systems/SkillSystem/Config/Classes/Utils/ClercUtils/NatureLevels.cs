using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleNatureLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Nature");
          player.oid.SetTextureOverride("clerc", "nature_domain");

          if (!player.windows.TryGetValue("spellSelection", out var cantrip1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Druid, 1, 0));
          else ((SpellSelectionWindow)cantrip1).CreateWindow(ClassType.Druid, 1, 0);

          List<int> skillList = new() { CustomSkill.AnimalHandlingProficiency, CustomSkill.NatureProficiency, CustomSkill.SurvivalProficiency };

          if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 1));
          else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 1);

          player.learnableSkills.TryAdd(CustomSkill.ClercCharmePlanteEtAnimaux, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercCharmePlanteEtAnimaux], player));
          player.learnableSkills[CustomSkill.ClercCharmePlanteEtAnimaux].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercCharmePlanteEtAnimaux].source.Add(Category.Class);

          player.LearnAlwaysPreparedSpell(CustomSpell.AmitieAnimale, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.SpeakAnimal, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.CroissanceDepines, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Barkskin, CustomClass.Clerc);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.TempeteDeNeige, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.CroissanceVegetale, CustomClass.Clerc);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercAttenuationElementaire, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercAttenuationElementaire], player));
          player.learnableSkills[CustomSkill.ClercAttenuationElementaire].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercAttenuationElementaire].source.Add(Category.Class);

          break;

        case 7:

          player.LearnAlwaysPreparedSpell(CustomSpell.LianeAvide, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.DominateAnimal, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell(CustomSpell.MurDePierre, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.FleauDinsectes, CustomClass.Clerc);

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
