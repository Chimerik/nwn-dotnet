using System.Collections.Generic;
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
      player.menu.choices.Add(("Activer le mode toucher.", () => new TouchMode(player.oid)));
      player.menu.choices.Add(("Activer/Désactiver le mode marche.", () => new WalkMode(player.oid)));
      player.menu.choices.Add(("Modifier l'affichage de mon casque.", () => new DisplayHelm(player.oid)));
      player.menu.choices.Add(("Modifier l'affichage de ma cape.", () => new DisplayCloak(player.oid)));
      player.menu.choices.Add(("Examiner les environs.", () => new ExamineArea(player.oid)));
      player.menu.choices.Add(("Gérer mes grimoires.", () => new SpellBooks(player)));
      player.menu.choices.Add(("Gérer mes raccourcis.", () => new QuickBar(player)));
      //player.menu.choices.Add(("Dissiper les maladies (uniquement pour l'alpha).", () => new DispelDisease(player.oid)));
      if (player.bonusRolePlay >= 4)
        player.menu.choices.Add(("Recommander un joueur.", () => new CommendCommand(player)));
      player.menu.choices.Add(("Sauvegarder l'apparence d'un de mes objets.", () => new SaveItemAppearance(player.oid)));
      player.menu.choices.Add(("Restaurer l'apparence d'un de mes objets.", () => new LoadAppearance(player.oid)));
      player.menu.choices.Add(("Charger une description sur mon personnage.", () => new LoadPCDescription(player)));
      player.menu.choices.Add(("Renommer une de mes invocations.", () => new RenameSummon(player.oid)));
      player.menu.choices.Add(("Créer un contrat d'échange de ressources privé.", () => new ContractMenu(player)));
      player.menu.choices.Add(("Tenter de me débloquer du décor.", () => new Unstuck(player.oid)));
      player.menu.choices.Add(("Réinitialiser l'affichage de ma position.", () => new ResetPosition(player.oid)));
      player.menu.choices.Add(("Gérer l'affichage des couleurs du chat.", () => new ChatColors(player)));
      player.menu.choices.Add(("Obtenir ma clé CD publique.", () => new GetPublicKey(player.oid)));
      player.menu.choices.Add(("Supprimer mon personnage.", () => new DeleteCharacter(player)));
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
      if(ObjectPlugin.GetInt(player.oid.LoginCreature, "_MENU_HOTKEYS_SWAPPED") == 0)
        ObjectPlugin.SetInt(player.oid.LoginCreature, "_MENU_HOTKEYS_SWAPPED", 1, 1);
      else
        ObjectPlugin.DeleteInt(player.oid.LoginCreature, "_MENU_HOTKEYS_SWAPPED");

      QuickBarSlot swapQBS = PlayerPlugin.GetQuickBarSlot(player.oid.LoginCreature, 0);
      PlayerPlugin.SetQuickBarSlot(player.oid.LoginCreature, 0, PlayerPlugin.GetQuickBarSlot(player.oid.LoginCreature, 1));
      PlayerPlugin.SetQuickBarSlot(player.oid.LoginCreature, 1, swapQBS);
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
