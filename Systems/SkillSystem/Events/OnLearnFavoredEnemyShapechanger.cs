using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyShapechanger(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyShapechanger))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyShapechanger);

      return true;
    }
  }
}
