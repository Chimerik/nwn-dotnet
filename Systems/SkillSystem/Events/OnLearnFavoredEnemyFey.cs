using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyFey(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyFey))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyFey);

      return true;
    }
  }
}
