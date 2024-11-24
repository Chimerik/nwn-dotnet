using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnElementalist(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Elementaliste))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.Elementaliste);

      if (!player.windows.TryGetValue("elementalistChoice", out var value)) player.windows.Add("elementalistChoice", new ElementalistChoiceWindow(player, player.oid.LoginCreature.Level));
      else ((ElementalistChoiceWindow)value).CreateWindow(player.oid.LoginCreature.Level);

      List<NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence) < 20)
        abilities.Add(new("Intelligence", (int)Ability.Intelligence));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Wisdom) < 20)
        abilities.Add(new("Sagesse", (int)Ability.Wisdom));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma) < 20)
        abilities.Add(new("Charisme", (int)Ability.Charisma));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var stat)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)stat).CreateWindow(abilities);
      }

      return true;
    }
  }
}
