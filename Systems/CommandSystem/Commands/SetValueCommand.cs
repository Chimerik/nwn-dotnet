using System;
using System.Collections.Generic;
using System.Text;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSetValueCommand(ChatSystem.Context ctx, Options.Result options)
    {
      Player player;
      if (Players.TryGetValue(ctx.oSender, out player))
      {
        if (((string)options.positional[0]).Length != 0)
        {
          int value;
          if (Int32.TryParse((string)options.positional[0], out value))
          {
            player.setValue = value;
            return;
          }   
        }

        player.setValue = 0;
      }
    }
  }
}
