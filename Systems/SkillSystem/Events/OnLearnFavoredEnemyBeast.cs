using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyBeast(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyBeast))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyBeast);

      return true;
    }
  }
}
