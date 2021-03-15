using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRefillCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.PlayerName == "Chim")
        ModuleSystem.SpawnCollectableResources(0.0f);
    }
  }
}
