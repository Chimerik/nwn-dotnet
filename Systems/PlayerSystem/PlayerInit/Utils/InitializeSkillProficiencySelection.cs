using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeSkillProficiencySelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_IN_SKILL_PROFICIENCY_SELECTION").HasValue)
        {
          List<int> skillList = new();

          foreach (var skill in oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_IN_SKILL_PROFICIENCY_SELECTION").Value.Split("_"))
            skillList.Add(int.Parse(skill));

          if (!windows.TryGetValue("skillProficiencySelection", out var skill3)) windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(this, skillList, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_NB_SKILL_PROFICIENCY_SELECTION").Value));
          else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_NB_SKILL_PROFICIENCY_SELECTION").Value);
        }
      }
    }
  }
}
