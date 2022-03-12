using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      var resetOpt = (bool)options.named.GetValueOrDefault("reset");
      
      if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
      {
        if (resetOpt)
        {
          __HandleReset(player);
        }

        DrawCommandList(player);
      }
    }
    public static void DrawCommandList(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici la liste des commandes disponibles en jeu.");
      player.menu.choices.Add(("Personnaliser l'affichage du menu.", () => __DrawMenuConfigPage(player)));
      player.menu.choices.Add(("Créer un contrat d'échange de ressources privé.", () => new ContractMenu(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static void __DrawMenuConfigPage (PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Configuration de l'affichage du menu.");
      player.menu.choices.Add(("Intervertir options Monter et Descendre.", () => __HandleSwapHotkeys(player)));
      player.menu.choices.Add(("Deplacer vers la gauche.", () => __HandleMoveLeft(player)));
      player.menu.choices.Add(("Deplacer vers la droite.", () => __HandleMoveRight(player)));
      player.menu.choices.Add(("Deplacer vers le haut.", () => __HandleMoveUp(player)));
      player.menu.choices.Add(("Deplacer vers le bas.", () => __HandleMoveDown(player)));
      player.menu.choices.Add(("Réinitialiser la position du menu à la valeur par defaut", () => __HandleReset(player)));
      player.menu.choices.Add(("Retour", () => DrawCommandList(player)));
      player.menu.choices.Add(("Sauvegarder et quitter", () => __HandleSaveAndClose(player)));
      player.menu.Draw();
    }

    private static void __HandleSwapHotkeys(PlayerSystem.Player player)
    {
      ;
      if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_MENU_HOTKEYS_SWAPPED").HasNothing)
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_MENU_HOTKEYS_SWAPPED").Value = 1;
      else
        player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_MENU_HOTKEYS_SWAPPED").Delete();

      PlayerQuickBarButton swapQBS = player.oid.ControlledCreature.GetQuickBarButton(0);
      player.oid.ControlledCreature.SetQuickBarButton(0, player.oid.ControlledCreature.GetQuickBarButton(1));
      player.oid.ControlledCreature.SetQuickBarButton(1, swapQBS);
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
