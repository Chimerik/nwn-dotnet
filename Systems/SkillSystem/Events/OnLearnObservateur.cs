using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnObservateur(PlayerSystem.Player player, int customSkillId)
    {
      List <NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence) < 20)
        abilities.Add(new("Intelligence", (int)Ability.Intelligence));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Wisdom) < 20)
        abilities.Add(new("Sagesse", (int)Ability.Wisdom));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      List<int> tempList = new() { CustomSkill.InsightProficiency, CustomSkill.InvestigationProficiency, CustomSkill.InvestigationProficiency, CustomSkill.PerceptionProficiency };
      List<int> skillList = new();

      foreach (var skill in tempList)
        if (!player.learnableSkills.TryGetValue(skill + 1, out var expertise) || expertise.currentLevel < 1)
          skillList.Add(skill);

      if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 1, learningFeat: CustomSkill.Observateur));
      else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 1, learningFeat: CustomSkill.Observateur);

      return true;
    }
  }
}
