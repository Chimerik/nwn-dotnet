using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAgresseurSauvage(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AgresseurSauvage))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.AgresseurSauvage);


      EffectSystem.ApplyAgresseurSauvage(player.oid.LoginCreature);

      return true;
    }
  }
}
