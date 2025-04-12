using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnPourfendeur3(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.oid.LoginCreature.OnCreatureAttack -= RangerUtils.OnAttackPourfendeur3;
      player.oid.LoginCreature.OnCreatureAttack += RangerUtils.OnAttackPourfendeur3;

      return true;
    }
  }
}
