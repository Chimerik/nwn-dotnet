using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemRage(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)customSkillId, player.oid.LoginCreature.GetFeatRemainingUses(Feat.BarbarianRage));
      player.oid.LoginCreature.RemoveFeat(Feat.BarbarianRage);

      return true;
    }
  }
}
