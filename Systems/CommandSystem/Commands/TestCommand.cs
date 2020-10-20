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

      }
    }
  }
}
