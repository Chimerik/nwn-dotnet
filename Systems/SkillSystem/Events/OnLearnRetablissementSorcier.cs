using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRetablissementSorcier(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.OnCombatStatusChange -= EnsoUtils.OnCombatEnsoRecoverSource;
      player.oid.OnCombatStatusChange += EnsoUtils.OnCombatEnsoRecoverSource;

      return true;
    }
  }
}
