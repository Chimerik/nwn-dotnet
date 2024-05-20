using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyDragon(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyDragon))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyDragon);

      return true;
    }
  }
}
