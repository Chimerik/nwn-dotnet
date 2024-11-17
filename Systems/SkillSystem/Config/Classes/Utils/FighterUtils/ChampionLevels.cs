using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static void HandleChampionLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(8).SetPlayerOverride(player.oid, "Champion");
          player.oid.SetTextureOverride("fighter", "champion");

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionImprovedCritical, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionImprovedCritical], player));
          player.learnableSkills[CustomSkill.FighterChampionImprovedCritical].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterChampionImprovedCritical].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionAthleteAccompli, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionAthleteAccompli], player));
          player.learnableSkills[CustomSkill.FighterChampionAthleteAccompli].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterChampionAthleteAccompli].source.Add(Category.Class);

          break;

        case 7:

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionBonusCombatStyle, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionBonusCombatStyle], player));
          player.learnableSkills[CustomSkill.FighterChampionBonusCombatStyle].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterChampionBonusCombatStyle].source.Add(Category.Class);

          if (!player.windows.TryGetValue("fightingStyleSelection", out var style)) player.windows.Add("fightingStyleSelection", new FightingStyleSelectionWindow(player, CustomSkill.FighterChampion));
          else ((FightingStyleSelectionWindow)style).CreateWindow(CustomSkill.FighterChampion);


          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.ChampionGuerrierHeroique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ChampionGuerrierHeroique], player));
          player.learnableSkills[CustomSkill.ChampionGuerrierHeroique].LevelUp(player);
          player.learnableSkills[CustomSkill.ChampionGuerrierHeroique].source.Add(Category.Class);

          break;

        case 15:

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionImprovedCritical, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionImprovedCritical], player));
          player.learnableSkills[CustomSkill.FighterChampionImprovedCritical].LevelUp(player);

          break;

        case 18:

          player.learnableSkills.TryAdd(CustomSkill.FighterChampionUltimeSurvivant, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FighterChampionUltimeSurvivant], player));
          player.learnableSkills[CustomSkill.FighterChampionUltimeSurvivant].LevelUp(player);
          player.learnableSkills[CustomSkill.FighterChampionUltimeSurvivant].source.Add(Category.Class);

          player.oid.LoginCreature.OnHeartbeat -= FighterUtils.OnHeartbeatUltimeSurvivant;
          player.oid.LoginCreature.OnHeartbeat += FighterUtils.OnHeartbeatUltimeSurvivant;

          break;
      }
    }
  }
}
