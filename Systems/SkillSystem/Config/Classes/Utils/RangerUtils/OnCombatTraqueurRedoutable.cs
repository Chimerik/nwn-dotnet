using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnCombatTraqueurRedoutable(OnCombatStatusChange onStatus)
    {
      if (onStatus.CombatStatus == CombatStatus.EnterCombat)
      {
        onStatus.Player.LoginCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.TraqueurRedoutable, NwTimeSpan.FromRounds(1));
      }
    }
  }
}
