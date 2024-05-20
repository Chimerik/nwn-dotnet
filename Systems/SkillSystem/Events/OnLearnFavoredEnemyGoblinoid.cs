using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyGoblinoid(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyGoblinoid))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyGoblinoid);

      return true;
    }
  }
}
