using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnPourfendeur(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Pourfendeur))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.Pourfendeur);

      List <NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(new("Force", (int)Ability.Strength));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) < 20)
        abilities.Add(new("Constitution", (int)Ability.Constitution));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackPourfendeur;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackPourfendeur;

      player.oid.LoginCreature.OnCreatureDamage -= CreatureUtils.OnDamagePourfendeur;
      player.oid.LoginCreature.OnCreatureDamage += CreatureUtils.OnDamagePourfendeur;

      return true;
    }
  }
}
