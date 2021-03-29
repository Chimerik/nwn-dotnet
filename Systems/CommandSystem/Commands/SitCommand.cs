using System;
using NWN.Core;
using NWN.Core.NWNX;
using System.Numerics;
using System.Collections.Generic;
using NWN.API.Constants;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSitCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        player.menu.Close();
        player.menu.isOpen = true;

        PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, SitDown, ObjectTypes.All, MouseCursor.Action);
      }
    }
    private static async void SitDown(CursorTargetData selection)
    {
      if (!PlayerSystem.Players.TryGetValue(selection.Player, out PlayerSystem.Player player))
        return;

      await selection.Player.ActionMoveTo(API.Location.Create(selection.Player.Area, selection.TargetPos, selection.Player.Rotation));
      await selection.Player.PlayAnimation(Animation.LoopingSitChair, 1, true, TimeSpan.FromSeconds(99999999));

      player.LoadMenuQuickbar(QuickbarType.Sit);
    }
  }
}
