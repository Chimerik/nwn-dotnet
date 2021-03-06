﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelAoeCommand(ChatSystem.Context ctx, Options.Result options)
    {
      foreach (NwAreaOfEffect aoe in NwObject.FindObjectsOfType<NwAreaOfEffect>().Where(aoe => aoe.Creator == ctx.oSender.ControlledCreature))
        aoe.Destroy();
    }
  }
}
