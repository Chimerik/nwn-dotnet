using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemAspectGlouton(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackAspectGlouton;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackAspectGlouton;

      return true;
    }
  }
}
