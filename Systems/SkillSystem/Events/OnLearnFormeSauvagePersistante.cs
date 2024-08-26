using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFormeSauvagePersistante(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.OnCombatStatusChange -= DruideUtils.OnCombatDruideRecoverFormeSauvage;
      player.oid.OnCombatStatusChange += DruideUtils.OnCombatDruideRecoverFormeSauvage;

      return true;
    }
  }
}
