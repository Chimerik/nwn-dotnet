using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    private static void ExecuteRefillCommand(string prout)
    {
      ModuleSystem.module.SpawnCollectableResources(0.0f);
    }
  }
}
