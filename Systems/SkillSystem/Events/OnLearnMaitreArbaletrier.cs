using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMaitreArbaletrier(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.RapidReload));
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.PointBlankShot));
      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackCrossbowMaster;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackCrossbowMaster;
      return true;
    }
  }
}
