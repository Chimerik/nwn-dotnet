using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyConstruct(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyConstruct))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyConstruct);

      return true;
    }
  }
}
