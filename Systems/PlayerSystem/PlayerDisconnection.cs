using System;
using NWN.API;
using NWN.API.Events;
using NWN.Core.NWNX;

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

      onPCDisconnect.Player.LoginCreature.GetLocalVariable<int>("_DISCONNECTING").Value = 1;

      if (player.menu.isOpen)
        player.menu.Close();

      if(player.serializedQuickbar != null)
        player.UnloadMenuQuickbar();

      onPCDisconnect.Player.LoginCreature.VisualTransform.Rotation.X = 0.0f;
      onPCDisconnect.Player.LoginCreature.VisualTransform.Translation.X = 0.0f;
      onPCDisconnect.Player.LoginCreature.VisualTransform.Translation.Y = 0.0f;
      onPCDisconnect.Player.LoginCreature.VisualTransform.Translation.Z = 0.0f;

      player.OnKeydown -= player.menu.HandleMenuFeatUsed;

      Party.HandlePartyChange(onPCDisconnect.Player);

      if (!player.areaExplorationStateDictionnary.ContainsKey(player.oid.LoginCreature.Area.Tag))
        player.areaExplorationStateDictionnary.Add(player.oid.LoginCreature.Area.Tag, PlayerPlugin.GetAreaExplorationState(player.oid.LoginCreature, player.oid.LoginCreature.Area));
      else
        player.areaExplorationStateDictionnary[player.oid.LoginCreature.Area.Tag] = PlayerPlugin.GetAreaExplorationState(player.oid.LoginCreature, player.oid.LoginCreature.Area);
    }
  }
}
