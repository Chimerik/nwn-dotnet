using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnWildMagicRecuperation(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(customSkillId)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(customSkillId));

      player.oid.LoginCreature.SetFeatRemainingUses(NwFeat.FromFeatId(customSkillId), (byte)NativeUtils.GetCreatureProficiencyBonus(player.oid.LoginCreature));

      return true;
    }
  }
}
