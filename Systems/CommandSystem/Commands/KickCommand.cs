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
