using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyMonstruous(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyMonstrous))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyMonstrous);

      return true;
    }
  }
}
