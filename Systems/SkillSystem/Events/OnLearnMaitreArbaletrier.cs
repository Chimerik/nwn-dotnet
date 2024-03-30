using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMaitreArbaletrier(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(Feat.RapidReload);
      player.oid.LoginCreature.AddFeat(Feat.PointBlankShot);
      return true;
    }
  }
}
