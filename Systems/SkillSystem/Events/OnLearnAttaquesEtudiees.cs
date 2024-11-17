using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAttaquesEtudiees(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FighterAttaquesEtudiees))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.FighterAttaquesEtudiees);

      player.oid.LoginCreature.OnCreatureAttack -= FighterUtils.OnAttackAttaquesEtudiees;
      player.oid.LoginCreature.OnCreatureAttack += FighterUtils.OnAttackAttaquesEtudiees;

      return true;
    }
  }
}
