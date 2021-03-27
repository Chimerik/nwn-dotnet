using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteAreaTagCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (!ctx.oSender.IsDM && !ctx.oSender.IsDMPossessed && !ctx.oSender.IsPlayerDM && ctx.oSender.PlayerName != "Chim")
      {
        ctx.oSender.SendServerMessage("Cette commande ne peut être utilisée qu'en mode dm.", Color.ORANGE);
        return;
      }

      ctx.oSender.SendServerMessage($"Tag de {ctx.oSender.Area.Name} : {ctx.oSender.Area.Tag}");
    }
  }
}
