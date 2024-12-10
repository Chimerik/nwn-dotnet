using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnExpertiseRapiere(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.LearnClassSkill(CustomSkill.ExpertiseTranspercer);
      player.LearnClassSkill(CustomSkill.ExpertiseAffaiblissement);
      player.LearnClassSkill(CustomSkill.ExpertiseMoulinet);

      return true;
    }
  }
}
