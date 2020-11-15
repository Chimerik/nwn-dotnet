using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      var resetOpt = (bool)options.named.GetValueOrDefault("reset");
      
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (resetOpt)
        {
          __HandleReset(player);
        }

        __DrawConfigPage(player);
      }
    }

    private static void __DrawConfigPage (PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Configuration de l'affichage du menu.";
      player.menu.choices.Add(("Deplacer vers la gauche.", () => __HandleMoveLeft(player)));
      player.menu.choices.Add(("Deplacer vers la droite.", () => __HandleMoveRight(player)));
      player.menu.choices.Add(("Deplacer vers le haut.", () => __HandleMoveUp(player)));
      player.menu.choices.Add(("Deplacer vers le bas.", () => __HandleMoveDown(player)));
      player.menu.choices.Add(("Reset la position à la valeur par defaut", () => __HandleReset(player)));
      player.menu.choices.Add(("Sauvegarder et quitter", () => __HandleSaveAndClose(player)));
      player.menu.Draw();
    }

    private static void __HandleMoveLeft(PlayerSystem.Player player)
    {
      player.menu.originLeft -= 1;
      player.menu.Draw();
    }

    private static void __HandleMoveRight(PlayerSystem.Player player)
    {
      player.menu.originLeft += 1;
      player.menu.Draw();
    }

    private static void __HandleMoveUp(PlayerSystem.Player player)
    {
      player.menu.originTop -= 1;
      player.menu.Draw();
    }

    private static void __HandleMoveDown(PlayerSystem.Player player)
    {
      player.menu.originTop += 1;
      player.menu.Draw();
    }

    private static void __HandleReset(PlayerSystem.Player player)
    {
      player.menu.ResetConfig();
      player.menu.Draw();
    }

    private static void __HandleSaveAndClose(PlayerSystem.Player player)
    {
      player.menu.Close();
    }
  }
}
