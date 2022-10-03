using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;

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

      Utils.LogMessageToDMs($"{onPCDisconnect.Player.PlayerName} vient de se déconnecter {onPCDisconnect.Player.LoginCreature.Name} ({NwModule.Instance.PlayerCount - 1} joueur(s))");

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

      if(player.oid.LoginCreature.Area != null)
        if (!player.areaExplorationStateDictionnary.ContainsKey(player.oid.LoginCreature.Area.Tag))
          player.areaExplorationStateDictionnary.Add(player.oid.LoginCreature.Area.Tag, player.oid.GetAreaExplorationState(player.oid.LoginCreature.Area));
        else
          player.areaExplorationStateDictionnary[player.oid.LoginCreature.Area.Tag] = player.oid.GetAreaExplorationState(player.oid.LoginCreature.Area);

      /*if (player.TryGetOpenedWindow("bankStorage", out Player.PlayerWindow storageWindow))
        ((Player.BankStorageWindow)storageWindow).BankSave();*/

      Task waitDisconnection = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromMilliseconds(200));
        foreach(Player connectedPlayer in Players.Values.Where(p => p.pcState != Player.PcState.Offline && p.TryGetOpenedWindow("playerList", out Player.PlayerWindow playerListWindow)))
          ((Player.PlayerListWindow)connectedPlayer.windows["playerList"]).UpdatePlayerList();
      });

      Log.Info($"{onPCDisconnect.Player.LoginCreature.Name} disconnected.");
    }
  }
}
