namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void OnLoginRefreshPlayerList()
      {
        foreach (Player connectedPlayer in Players.Values)
          if (connectedPlayer.pcState != PcState.Offline && connectedPlayer.TryGetOpenedWindow("playerList", out PlayerWindow playerListWindow))
            ((PlayerListWindow)playerListWindow).UpdatePlayerList();
      }
    }
  }
}
