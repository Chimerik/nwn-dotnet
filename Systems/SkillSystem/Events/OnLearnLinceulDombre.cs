using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRadianceDeLaube(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercRadianceDeLaube))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercRadianceDeLaube);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercRadianceDeLaube, 1);

      return true;
    }
  }
}
