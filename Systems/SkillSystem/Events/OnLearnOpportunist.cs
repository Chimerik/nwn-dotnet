using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnOpportunist(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MonkOpportuniste))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MonkOpportuniste);

      player.oid.LoginCreature.OnCreatureAttack -= MonkUtils.OnAttackMonkOpportunist;
      player.oid.LoginCreature.OnCreatureAttack += MonkUtils.OnAttackMonkOpportunist;

      return true;
    }
  }
}
