using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerLeave(OnClientDisconnect onPCDisconnect)
    {
      if (onPCDisconnect.Player is null || onPCDisconnect.Player.LoginCreature is null)
        return;
      
      LogUtils.LogMessage($"{onPCDisconnect.Player.PlayerName} vient de se déconnecter {onPCDisconnect.Player.LoginCreature.Name} ({NwModule.Instance.PlayerCount - 1} joueur(s))", LogUtils.LogType.PlayerConnections);
      onPCDisconnect.Player.LoginCreature.GetObjectVariable<PersistentVariableInt>("_PLAYER_HP").Value = onPCDisconnect.Player.LoginCreature.HP;

      if (!Players.TryGetValue(onPCDisconnect.Player.LoginCreature, out Player player))
        return;

      player.pcState = Player.PcState.Offline;

      if (player.menu.isOpen)
        player.menu.Close();

      if(player.serializedQuickbar is not null)
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

      var polymorph = player.oid.LoginCreature.ActiveEffects.FirstOrDefault(e => e.EffectType == EffectType.Polymorph);

      if (polymorph is not null)
      {
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_SAVED_SHAPE").Value = polymorph.IntParams[0];
        EffectUtils.RemoveEffectType(player.oid.LoginCreature, EffectType.Polymorph);
      }

      /*if (player.TryGetOpenedWindow("bankStorage", out Player.PlayerWindow storageWindow))
        ((Player.BankStorageWindow)storageWindow).BankSave();*/

      Task waitDisconnection = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromMilliseconds(200));
        foreach(Player connectedPlayer in Players.Values.Where(p => p.pcState != Player.PcState.Offline && p.TryGetOpenedWindow("playerList", out Player.PlayerWindow playerListWindow)))
          ((Player.PlayerListWindow)connectedPlayer.windows["playerList"]).UpdatePlayerList();
      });
    }
  }
}
