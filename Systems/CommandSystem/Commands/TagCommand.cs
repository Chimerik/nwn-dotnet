using System;
using NWN.Core;
using System.Numerics;
using NWN.Services;
using NWN.API;
using NWN.API.Constants;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTagCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.IsDM || ctx.oSender.IsDMPossessed || ctx.oSender.IsPlayerDM)
      {
        ctx.oSender.GetLocalVariable<string>("_RENAME_VALUE").Value = (string)options.positional[0];
        PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, ChangeTagTarget, ObjectTypes.All, MouseCursor.Create);
      }
      else
        ctx.oSender.SendServerMessage("Il s'agit d'une commande DM, vous ne pouvez pas en faire usage en PJ.", Color.ORANGE);
    }
    private static void ChangeTagTarget(CursorTargetData selection)
    {
      if (selection.TargetObj != null)
        selection.TargetObj.Tag = selection.Player.GetLocalVariable<string>("_RENAME_VALUE").Value;
      else
        selection.Player.SendServerMessage("Veuillez sélectionner une cible valide.", Color.RED);
    }
  }
}
