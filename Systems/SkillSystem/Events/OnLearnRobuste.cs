using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRobuste(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.Toughness))
        player.oid.LoginCreature.AddFeat(Feat.Toughness);

      return true;
    }
  }
}
