﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class BardUtils
  {
    public static void OnCombatBardRecoverInspiration(OnCombatStatusChange onStatus)
    {
      if (onStatus.CombatStatus == CombatStatus.EnterCombat && onStatus.Player.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.BardInspiration) < 2)
        onStatus.Player.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BardInspiration, (byte)(onStatus.Player.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.BardInspiration) + 2));
    }
  }
}
