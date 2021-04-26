using NWN.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerLeave(OnClientDisconnect onPCDisconnect)
    {
      if (onPCDisconnect.Player == null)
        return;

      Log.Info($"{onPCDisconnect.Player.Name} disconnecting.");

      if (!Players.TryGetValue(onPCDisconnect.Player, out Player player))
        return;

      onPCDisconnect.Player.GetLocalVariable<int>("_DISCONNECTING").Value = 1;

      if (player.menu.isOpen)
        player.menu.Close();

      player.UnloadMenuQuickbar();

      onPCDisconnect.Player.VisualTransform.Rotation.X = 0.0f;
      onPCDisconnect.Player.VisualTransform.Translation.X = 0.0f;
      onPCDisconnect.Player.VisualTransform.Translation.Y = 0.0f;
      onPCDisconnect.Player.VisualTransform.Translation.Z = 0.0f;
      player.setValue = Config.invalidInput;
      player.setString = "";
      player.OnKeydown -= player.menu.HandleMenuFeatUsed;

      if (!player.areaExplorationStateDictionnary.ContainsKey(player.oid.Area.Tag))
        player.areaExplorationStateDictionnary.Add(player.oid.Area.Tag, PlayerPlugin.GetAreaExplorationState(player.oid, player.oid.Area));
      else
        player.areaExplorationStateDictionnary[player.oid.Area.Tag] = PlayerPlugin.GetAreaExplorationState(player.oid, player.oid.Area);
    }
  }
}
