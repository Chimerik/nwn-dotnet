using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnBarbarianFastMovement(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.BarbarianEndurance))
        player.oid.LoginCreature.AddFeat(Feat.BarbarianEndurance);

      return true;
    }
  }
}
