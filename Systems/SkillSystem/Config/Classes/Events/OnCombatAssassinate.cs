using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class RogueUtils
  {
    public static void OnCombatAssassinate(OnCombatStatusChange onStatus)
    {
      if (onStatus.CombatStatus == CombatStatus.EnterCombat)
      {
        onStatus.Player.LoginCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.Assassinate, TimeSpan.FromSeconds(5));
        onStatus.Player.LoginCreature.OnDamaged -= OnDamagedRemoveAssassinate;
        onStatus.Player.LoginCreature.OnDamaged += OnDamagedRemoveAssassinate;
      }
    }
  }
}
