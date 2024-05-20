using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyOutsider(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyOutsider))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyOutsider);

      return true;
    }
  }
}
