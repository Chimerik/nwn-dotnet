using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFavoredEnemyMagicalBeast(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.FavoredEnemyMagicalBeast))
        player.oid.LoginCreature.AddFeat(Feat.FavoredEnemyMagicalBeast);

      return true;
    }
  }
}
