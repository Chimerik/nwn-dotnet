using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyUndead(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyUndead))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyUndead);

      return true;
    }
  }
}
