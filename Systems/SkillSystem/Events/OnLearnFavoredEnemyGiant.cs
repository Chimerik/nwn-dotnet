using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyGiant(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyGiant))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyGiant);

      return true;
    }
  }
}
