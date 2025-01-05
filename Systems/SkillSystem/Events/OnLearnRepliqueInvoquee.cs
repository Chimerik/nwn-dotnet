using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRepliqueInvoquee(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MoveRepliqueDuplicite))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MoveRepliqueDuplicite);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)customSkillId, player.oid.LoginCreature.GetFeatRemainingUses(Feat.TurnUndead));
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.MoveRepliqueDuplicite, 0);

      return true;
    }
  }
}
