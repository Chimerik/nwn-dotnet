using System;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteKickCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.IsDM || ctx.oSender.PlayerName == "Chim")
      {
        if(ctx.oTarget.PlayerName != "Chim")
          ctx.oTarget.BootPlayer();
      }
    }
  }
}
