using System;
using System.Collections.Generic;
using NWN.Enums;
using NWN.NWNX;
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
        int prout = HashCode.Combine<MineralType, ItemSystem.ItemCategory>(MineralType.Pyerite, ItemSystem.ItemCategory.Shield);
        foreach (ItemProperty ip in test[prout])
        {
          player.SendMessage($"code : {prout}");
        }
      }
    }
  }
}
