using System.Collections.Generic;
using System.Security.Cryptography;
using Anvil.API;
using Discord;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnBroyeur(PlayerSystem.Player player, int customSkillId)
    {
      List<Ability> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(Ability.Strength);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) < 20)
        abilities.Add(Ability.Constitution);

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Broyeur)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.Broyeur));

      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackBroyeur;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackBroyeur;

      return true;
    }
  }
}
