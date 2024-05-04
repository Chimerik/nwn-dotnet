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
        LogUtils.LogMessage("------------- Assassinat - Evénement d'entrée en combat -----------------", LogUtils.LogType.Combat);

        onStatus.Player.LoginCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.Assassinate, TimeSpan.FromSeconds(5));
        onStatus.Player.LoginCreature.OnDamaged -= OnDamagedRemoveAssassinate;
        onStatus.Player.LoginCreature.OnDamaged += OnDamagedRemoveAssassinate;
      }
      else
        LogUtils.LogMessage("------------- Assassinat - Evénement de sortie de combat ----------------", LogUtils.LogType.Combat);
    }
  }
}
