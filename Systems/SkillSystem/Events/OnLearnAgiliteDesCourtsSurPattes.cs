using System.Collections.Generic;
using System.Security.Cryptography;
using Anvil.API;
using Discord;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAgiliteDesCourtsSurPattes(PlayerSystem.Player player, int customSkillId)
    {
      List<Ability> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(Ability.Strength);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
        abilities.Add(Ability.Dexterity);

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      if (player.learnableSkills.TryGetValue(CustomSkill.AcrobaticsProficiency, out LearnableSkill acrobatics))
      {
        if (acrobatics.currentLevel < 1)
          acrobatics.LevelUp(player);
      }
      else
      {
        player.learnableSkills.Add(CustomSkill.AcrobaticsProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AcrobaticsProficiency]));
        player.learnableSkills[CustomSkill.AcrobaticsProficiency].LevelUp(player);
      }

      foreach (var eff in player.oid.LoginCreature.ActiveEffects)
        if(eff.Tag == EffectSystem.DwarfSlowEffectTag)
          player.oid.LoginCreature.RemoveEffect(eff);

      return true;
    }
  }
}
