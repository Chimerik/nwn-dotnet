using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyAberration(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyAberration))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyAberration);

      return true;
    }
  }
}
