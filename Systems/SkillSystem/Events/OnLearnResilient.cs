using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnResilient(PlayerSystem.Player player, int customSkillId)
    {
      List<Ability> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20 
        || (player.learnableSkills.TryGetValue(CustomSkill.StrengthSavesProficiency, out LearnableSkill str) && str.currentLevel < 1))
        abilities.Add(Ability.Strength);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20
        || (player.learnableSkills.TryGetValue(CustomSkill.DexteritySavesProficiency, out LearnableSkill dex) && dex.currentLevel < 1))
        abilities.Add(Ability.Dexterity);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) < 20
        || (player.learnableSkills.TryGetValue(CustomSkill.ConstitutionSavesProficiency, out LearnableSkill con) && con.currentLevel < 1))
        abilities.Add(Ability.Constitution);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence) < 20
        || (player.learnableSkills.TryGetValue(CustomSkill.IntelligenceSavesProficiency, out LearnableSkill intel) && intel.currentLevel < 1))
        abilities.Add(Ability.Intelligence);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Wisdom) < 20
        || (player.learnableSkills.TryGetValue(CustomSkill.WisdomSavesProficiency, out LearnableSkill wis) && wis.currentLevel < 1))
        abilities.Add(Ability.Wisdom);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma) < 20
        || (player.learnableSkills.TryGetValue(CustomSkill.CharismaSavesProficiency, out LearnableSkill cha) && cha.currentLevel < 1))
        abilities.Add(Ability.Charisma);

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities, true));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities, true);
      }

      return true;
    }
  }
}
