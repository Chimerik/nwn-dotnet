using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnBroyeur(PlayerSystem.Player player, int customSkillId)
    {
      List<NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(new("Force", (int)Ability.Strength));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) < 20)
        abilities.Add(new("Constitution", (int)Ability.Constitution));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Broyeur))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.Broyeur);

      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackBroyeur;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackBroyeur;

      return true;
    }
  }
}
