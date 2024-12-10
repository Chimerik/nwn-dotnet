using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnExpertiseKatana(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.LearnClassSkill(CustomSkill.ExpertiseCharge);
      player.LearnClassSkill(CustomSkill.ExpertiseLaceration);
      player.LearnClassSkill(CustomSkill.ExpertisePreparation);

      return true;
    }
  }
}
