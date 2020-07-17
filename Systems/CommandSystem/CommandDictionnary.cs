using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static Dictionary<string, Command> commandDic = new Dictionary<string, Command>
    {
      {
        "help", new Command(
          name: "help",
          description: new Command.Description(title: "Display the list of all available commands."),
          execute: ExecuteHelpCommand
        )
      },
      {
        "frostattack",
        new Command(
          name: "frostattack",
          description: new Command.Description(title: "todo"),
          execute: ExecuteFrostAttackCommand
        )
      },
      {
        "walk",
        new Command(
          name: "walk",
          description: new Command.Description(title: "todo"),
          execute: ExecuteWalkCommand
        )
      },
      {
        "reveal",
        new Command(
          name: "reveal",
          description: new Command.Description(title: "Permet de sortir du mode furtif aux yeux du groupe ou de la cible"),
          execute: ExecuteRevealCommand
        )
      },
      {
        "dispel_aoe",
        new Command(
          name: "dispel_aoe",
          description: new Command.Description(title: "Dissipe vos effets d'AoE dans la zone actuelle"),
          execute: ExecuteDispelAoeCommand
        )
      },
      {
        "dispel",
        new Command(
          name: "dispel",
          description: new Command.Description(title: "Dissipe les effets de sorts créés par vous sur vous-même ou sur la cible"),
          execute: ExecuteDispelCommand
        )
      },
      {
        "dissip",
        new Command(
          name: "dissip",
          description: new Command.Description(title: "Dissipe les effets de sorts créés par vous sur vous-même ou sur la cible"),
          execute: ExecuteDispelCommand
        )
      },
      {
        "invi",
        new Command(
          name: "invi",
          description: new Command.Description(title: "Dissipe tout effet d'invisibilité actif sur  vous"),
          execute: ExecuteDispelInviCommand
        )
      },
      {
        "casque",
        new Command(
          name: "casque",
          description: new Command.Description(title: "Active/Désactive l'affichage de votre casque"),
          execute: ExecuteDisplayHelmCommand
        )
      },
      {
        "cape",
        new Command(
          name: "cape",
          description: new Command.Description(title: "Active/Désactive l'affichage de votre cape"),
          execute: ExecuteDisplayCloakCommand
        )
      },
      {
        "touch",
        new Command(
          name: "touch",
          description: new Command.Description(title: "Active/Désactive le mode toucher (évite les collisions entre personnages)"),
          execute: ExecuteTouchCommand
        )
      },
      {
        "reboot",
        new Command(
          name: "reboot",
          description: new Command.Description(title: "Permet de redémarrer le module (bas les pattes, vils joueurs) !)"),
          execute: ExecuteRebootCommand
        )
      },
      {
        "persist",
        new Command(
          name: "persist",
          description: new Command.Description(title: "Active/Désactive le système de persistance des placeables créés par DM"),
          execute: ExecutePlaceablePersistanceCommand
        )
      },
    };
  }
}
