using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnExpertiseFouet(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.LearnClassSkill(CustomSkill.ExpertiseDesarmement);
      player.LearnClassSkill(CustomSkill.ExpertiseDestabiliser);
      player.LearnClassSkill(CustomSkill.ExpertiseAffaiblissement);
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseDesarmement))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseDesarmement);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseDestabiliser))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseDestabiliser);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseAffaiblissement))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseAffaiblissement);

      return true;
    }
  }
}
