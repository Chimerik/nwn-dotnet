using System;
using Anvil.API;
using Anvil.API.Events;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void StartIntroMirrorDialog(PlaceableEvents.OnLeftClick onUsed)
    {
      if (Players.TryGetValue(onUsed.ClickedBy.LoginCreature, out Player player))
      {
        if (!player.windows.ContainsKey("introMirror")) player.windows.Add("introMirror", new Player.IntroMirrorWindow(player));
        else ((Player.IntroMirrorWindow)player.windows["introMirror"]).CreateWindow();
      }
    }
  }
}
