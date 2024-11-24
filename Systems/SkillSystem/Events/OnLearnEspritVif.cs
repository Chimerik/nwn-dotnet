using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnEspritVif(PlayerSystem.Player player, int customSkillId)
    {
      byte rawIntelligence = player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence);
      if (rawIntelligence < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Intelligence, (byte)(rawIntelligence + 1));

      List<int> tempList = new() { CustomSkill.ArcanaProficiency, CustomSkill.HistoryProficiency, CustomSkill.InvestigationProficiency, CustomSkill.NatureProficiency, CustomSkill.ReligionProficiency };
      List<int> skillList = new();

      foreach (var skill in tempList)
        if (!player.learnableSkills.TryGetValue(skill + 1, out var expertise) || expertise.currentLevel < 1)
          skillList.Add(skill);

      if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 1, learningFeat: CustomSkill.EspritVif));
      else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 1, learningFeat: CustomSkill.EspritVif);

      return true;
    }
  }
}
