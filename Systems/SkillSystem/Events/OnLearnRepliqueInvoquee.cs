using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRepliqueInvoquee(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercRepliqueInvoquee))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercRepliqueInvoquee);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercRepliqueInvoquee, player.oid.LoginCreature.GetFeatRemainingUses(Feat.TurnUndead));

      return true;
    }
  }
}
