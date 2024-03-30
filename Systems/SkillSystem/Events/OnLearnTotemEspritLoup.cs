using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemEspritLoup(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TotemHurlementGalvanisant))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.TotemHurlementGalvanisant);

      return true;
    }
  }
}
