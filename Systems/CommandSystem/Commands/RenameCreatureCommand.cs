using System;
using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRenameCreatureCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        player.lastTargetedCommandUsed = "renameCreature";
        player.lastTargetedCommandArgument = (string)options.positional[0];

        //NWScript.EnterTargetingMode(player, ObjectType.Creature);
        NWScript.ExecuteScript("on_pc_target", player); // bouchon en attendant d'avoir la vraie fonction
      }
    }
  }
}
