using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRobuste(PlayerSystem.Player player, int customSkillId)
    {
      foreach (var level in player.oid.LoginCreature.LevelInfo)
        level.HitDie += 2;

      return true;
    }
  }
}
