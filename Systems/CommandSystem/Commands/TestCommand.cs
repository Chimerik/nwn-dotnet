using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.CollectSystem;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        Console.WriteLine($"uint player {ctx.oSender}");
        int convertedUint = Convert.ToInt32(ctx.oSender);
        NWScript.SetLocalInt(ctx.oSender, "_PC_ID", convertedUint);
        Console.WriteLine($"int player {NWScript.GetLocalInt(ctx.oSender, "_PC_ID")}");
        Console.WriteLine($"uint player converted {Convert.ToUInt32(NWScript.GetLocalInt(ctx.oSender, "_PC_ID"))}");
      }
    }
  }
}
