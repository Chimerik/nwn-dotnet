using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnInspirationSuperieure(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.OnCombatStatusChange -= BardUtils.OnCombatBardRecoverInspiration;
      player.oid.OnCombatStatusChange += BardUtils.OnCombatBardRecoverInspiration;

      return true;
    }
  }
}
