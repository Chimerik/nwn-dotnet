using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnBarbarianRage(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.BarbarianRage))
        player.oid.LoginCreature.AddFeat(Feat.BarbarianRage);

      player.oid.LoginCreature.SetFeatRemainingUses(Feat.BarbarianRage, 2);

      return true;
    }
  }
}
