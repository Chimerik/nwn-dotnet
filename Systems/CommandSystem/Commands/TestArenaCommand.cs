namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestArenaCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        Arena.Menu.Draw(player);        
      }
    }
  }
}
