using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteContractMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
        new PrivateContractCreator(player);
    }
  }
}
