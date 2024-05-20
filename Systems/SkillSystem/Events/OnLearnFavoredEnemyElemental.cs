using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyElemental(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyElemental))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyElemental);

      return true;
    }
  }
}
