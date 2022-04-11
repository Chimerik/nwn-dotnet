using Anvil.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDMMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player) && (ctx.oSender.IsDM || ctx.oSender.PlayerName == "Chim"))
      {
        DrawDMCommandList(player);
      }
      else
        ctx.oSender.SendServerMessage("Il faut être connecté en DM pour avoir accès à ce menu.", ColorConstants.Orange);
    }
    public static void DrawDMCommandList(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici la liste des commandes disponibles en jeu.");
      player.menu.choices.Add(("Afficher le menu d'édition des pnjs.", () => new PNJFactory(player)));
     
      player.menu.Draw();
    }
  }
}
