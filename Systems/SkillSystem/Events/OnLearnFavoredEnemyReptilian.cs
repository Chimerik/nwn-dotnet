using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyReptilian(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyReptilian))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyReptilian);

      return true;
    }
  }
}
