using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyOrc(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyOrc))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyOrc);

      return true;
    }
  }
}
