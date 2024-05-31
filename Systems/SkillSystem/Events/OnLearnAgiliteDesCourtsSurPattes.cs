using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAgiliteDesCourtsSurPattes(PlayerSystem.Player player, int customSkillId)
    {
      List <NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(new("Force", (int)Ability.Strength));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
        abilities.Add(new("Dextérité", (int)Ability.Dexterity));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      List<int> skillList = new() { CustomSkill.AcrobaticsProficiency, CustomSkill.AthleticsProficiency };

      if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 1));
      else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 1);

      EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.DwarfSlowEffectTag);

      return true;
    }
  }
}
