using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnExpertiseHallebarde(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.LearnClassSkill(CustomSkill.ExpertiseRenforcement);
      player.LearnClassSkill(CustomSkill.ExpertiseCharge);
      player.LearnClassSkill(CustomSkill.ExpertiseFendre);

      return true;
    }
  }
}
