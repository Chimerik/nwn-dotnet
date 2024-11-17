using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static void HandleWarMasterLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(8).SetPlayerOverride(player.oid, "Maître de Guerre");
          player.oid.SetTextureOverride("fighter", "warmaster");

          List<int> skillList = new();

          foreach (var skill in startingPackage.skillChoiceList)
              if(!player.learnableSkills.ContainsKey(skill.id))
                skillList.Add(skill.id);

          if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 1));
          else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 1);

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Value = 3;
          player.InitializeManoeuvreChoice();

          break;

        case 7:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Value = 2;
          player.InitializeManoeuvreChoice();

          player.learnableSkills.TryAdd(CustomSkill.WarMasterConnaisTonEnnemi, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarMasterConnaisTonEnnemi], player));
          player.learnableSkills[CustomSkill.WarMasterConnaisTonEnnemi].LevelUp(player);
          player.learnableSkills[CustomSkill.WarMasterConnaisTonEnnemi].source.Add(Category.Class);

          break;

        case 11:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Value = 2;
          player.InitializeManoeuvreChoice();

          break;

        case 15:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Value = 2;
          player.InitializeManoeuvreChoice();

          player.oid.OnCombatStatusChange -= FighterUtils.OnCombatWarMasterRecoverManoeuvre;
          player.oid.OnCombatStatusChange += FighterUtils.OnCombatWarMasterRecoverManoeuvre;

          break;

      }
    }
  }
}
