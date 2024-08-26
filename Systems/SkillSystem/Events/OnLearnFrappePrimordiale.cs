using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFrappePrimordiale(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DruideFrappePrimordialeFroid))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DruideFrappePrimordialeFroid);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DruideFrappePrimordialeFeu))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DruideFrappePrimordialeFeu);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DruideFrappePrimordialeElec))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DruideFrappePrimordialeElec);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DruideFrappePrimordialeTonnerre))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DruideFrappePrimordialeTonnerre);

      return true;
    }
  }
}
