using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteStuckCommand(ChatSystem.Context ctx, Options.Result options)
    {
      NWScript.JumpToLocation(NWScript.GetLocation(ctx.oSender));
      NWScript.SendMessageToPC(ctx.oSender, "Tentative de déblocage !");
    }
  }
}
