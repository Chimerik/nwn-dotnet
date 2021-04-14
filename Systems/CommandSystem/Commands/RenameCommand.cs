using System;
using NWN.Core;
using NWN.Services;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRenameCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.IsDM || ctx.oSender.IsDMPossessed || ctx.oSender.IsPlayerDM)
      {
        ctx.oSender.GetLocalVariable<string>("_RENAME_VALUE").Value = (string)options.positional[0];
        PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, RenameTarget, ObjectTypes.All, MouseCursor.Create);
      }
      else
        ctx.oSender.SendServerMessage("Il s'agit d'une commande DM, vous ne pouvez pas en faire usage en PJ.", Color.ORANGE);
    }
    private static void RenameTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.TargetObject != null)
        selection.TargetObject.Name = selection.Player.GetLocalVariable<string>("_RENAME_VALUE").Value;
      else
        selection.Player.SendServerMessage("Veuillez sélectionner une cible valide.", Color.RED);
    }
  }
}
