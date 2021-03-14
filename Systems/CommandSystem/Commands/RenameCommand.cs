using System;
using NWN.Core;
using NWN.Services;
using NWN.API;
using NWN.API.Constants;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRenameCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        if (player.oid.IsDM || player.oid.IsDMPossessed || player.oid.IsPlayerDM)
        {
          player.oid.GetLocalVariable<string>("_RENAME_VALUE").Value = (string)options.positional[0];
          PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, RenameTarget, ObjectTypes.All, MouseCursor.Create);
        }
        else
          player.oid.SendServerMessage("Il s'agit d'une commande DM, vous ne pouvez pas en faire usage en PJ.", Color.ORANGE);
      }
    }
    private static void RenameTarget(CursorTargetData selection)
    {
      if (selection.TargetObj != null)
        selection.TargetObj.Name = selection.Player.GetLocalVariable<string>("_RENAME_VALUE").Value;
      else
        selection.Player.SendServerMessage("Veuillez sélectionner une cible valide.", Color.RED);
    }
  }
}
