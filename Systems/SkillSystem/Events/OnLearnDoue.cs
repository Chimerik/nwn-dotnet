using System.Collections.Generic;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDoue(PlayerSystem.Player player, int customSkillId)
    {
      List<int> skillList = new() { CustomSkill.AcrobaticsProficiency, CustomSkill.AnimalHandlingProficiency, CustomSkill.ArcanaProficiency, CustomSkill.AthleticsProficiency, CustomSkill.DeceptionProficiency, CustomSkill.HistoryProficiency, CustomSkill.InsightProficiency, CustomSkill.IntimidationProficiency, CustomSkill.InvestigationProficiency, CustomSkill.MedicineProficiency, CustomSkill.NatureProficiency, CustomSkill.PerceptionProficiency, CustomSkill.PerformanceProficiency, CustomSkill.PersuasionProficiency, CustomSkill.ReligionProficiency, CustomSkill.SleightOfHandProficiency, CustomSkill.StealthProficiency, CustomSkill.SurvivalProficiency };

      if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 3, learningFeat: customSkillId));
      else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 3, learningFeat: customSkillId);

      return true;
    }
  }
}
