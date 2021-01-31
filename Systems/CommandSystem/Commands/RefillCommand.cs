using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRefillCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender) == 1)
      {
        ModuleSystem.module.SpawnCollectableResources(0.0f);
      }
    }
  }
}
