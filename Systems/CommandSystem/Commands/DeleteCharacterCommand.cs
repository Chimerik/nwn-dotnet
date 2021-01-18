using System;
using NWN.Core;
using System.Numerics;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDeleteCharacterCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        AdminPlugin.DeletePlayerCharacter(player.oid, 1, $"Le personnage {NWScript.GetName(player.oid)} a été supprimé.");
      }
    }
  }
}
