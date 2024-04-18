using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class RogueUtils
  {
    public static void OnCombatThiefReflex(OnCombatStatusChange onStatus)
    {
      if (onStatus.CombatStatus == CombatStatus.EnterCombat)
        onStatus.Player.LoginCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.Assassinate, NwTimeSpan.FromRounds(1));
    }
  }
}
