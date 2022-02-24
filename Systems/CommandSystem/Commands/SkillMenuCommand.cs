namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSkillMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
      {
        if (player.windows.ContainsKey("learnables"))
          ((PlayerSystem.Player.LearnableWindow)player.windows["learnables"]).CreateWindow();
        else
          player.windows.Add("learnables", new PlayerSystem.Player.LearnableWindow(player));
      }
    }
  }
}
