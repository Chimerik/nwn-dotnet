using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFrappeGuidee(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercFrappeGuidee))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercFrappeGuidee);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercFrappeGuidee, player.oid.LoginCreature.GetFeatRemainingUses(Feat.TurnUndead));

      return true;
    }
  }
}
