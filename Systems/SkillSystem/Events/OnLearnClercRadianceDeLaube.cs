using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnClercRadianceDeLaube(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercRadianceDeLaube))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercRadianceDeLaube);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercRadianceDeLaube, player.oid.LoginCreature.GetFeatRemainingUses(Feat.TurnUndead));

      return true;
    }
  }
}
