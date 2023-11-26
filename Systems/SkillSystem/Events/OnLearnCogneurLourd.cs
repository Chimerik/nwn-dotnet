using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnCogneurLourd(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(customSkillId));
      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackCogneurLourd;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackCogneurLourd;
      return true;
    }
  }
}
