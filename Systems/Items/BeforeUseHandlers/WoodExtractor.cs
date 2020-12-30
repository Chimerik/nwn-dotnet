using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Items.BeforeUseHandlers
{
  public static class WoodExtractor
  {
    public static void HandleActivate(uint oItem, Player player, uint oTarget)
    {
      if (NWScript.GetDistanceBetween(player.oid, oTarget) > 5.0f)
      {
        NWScript.SendMessageToPC(player.oid, $"Vous êtes trop éloigné de votre cible pour démarrer l'extraction.");
        return;
      }

      NWScript.SendMessageToPC(player.oid, "Work In Progress");
    }
  }
}
