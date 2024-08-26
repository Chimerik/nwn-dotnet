
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void OnCombatDruideRecoverFormeSauvage(OnCombatStatusChange onStatus)
    {
      if (onStatus.CombatStatus == CombatStatus.EnterCombat 
        && onStatus.Player.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.DruideCompagnonSauvage) < 1)
          RestoreFormeSauvage(onStatus.Player.LoginCreature, 1);
    }
  }
}
