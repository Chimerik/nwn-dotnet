﻿using System.Linq;
using System.Numerics;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerLeave(OnClientDisconnect onPCDisconnect)
    {
      if (onPCDisconnect.Player == null || onPCDisconnect.Player.LoginCreature == null)
        return;

      Log.Info($"{onPCDisconnect.Player.LoginCreature.Name} disconnecting.");

      if (!Players.TryGetValue(onPCDisconnect.Player.LoginCreature, out Player player))
        return;

      player.pcState = Player.PcState.Offline;

      if (player.menu.isOpen)
        player.menu.Close();

      if(player.serializedQuickbar != null)
        player.UnloadMenuQuickbar();

      onPCDisconnect.Player.LoginCreature.VisualTransform.Translation = new Vector3(0, 0, 0);
      onPCDisconnect.Player.LoginCreature.VisualTransform.Rotation = new Vector3(0, 0, 0);

      player.OnKeydown -= player.menu.HandleMenuFeatUsed;

      Party.HandlePartyChange(onPCDisconnect.Player);

      if (!player.areaExplorationStateDictionnary.ContainsKey(player.oid.LoginCreature.Area.Tag))
        player.areaExplorationStateDictionnary.Add(player.oid.LoginCreature.Area.Tag, player.oid.GetAreaExplorationState(player.oid.LoginCreature.Area));
      else
        player.areaExplorationStateDictionnary[player.oid.LoginCreature.Area.Tag] = player.oid.GetAreaExplorationState(player.oid.LoginCreature.Area);

      if (player.openedWindows.ContainsKey("bankStorage") && player.windows.ContainsKey("bankStorage"))
        ((Player.BankStorageWindow)player.windows["bankStorage"]).BankSave();

      Log.Info($"{onPCDisconnect.Player.LoginCreature.Name} disconnected.");
    }
  }
}
