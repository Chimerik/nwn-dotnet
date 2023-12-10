using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnPourfendeur(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Pourfendeur)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.Pourfendeur));

      List <Ability> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(Ability.Strength);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) < 20)
        abilities.Add(Ability.Constitution);

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
