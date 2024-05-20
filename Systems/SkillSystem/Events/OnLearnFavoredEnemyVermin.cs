using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyVermin(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyVermin))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyVermin);

      return true;
    }
  }
}
