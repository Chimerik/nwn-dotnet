using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnEsquiveTotale(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(Feat.ImprovedEvasion))
        player.oid.LoginCreature.AddFeat(Feat.ImprovedEvasion);

      return true;
    }
  }
}
