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
          description: new Command.Description(
            title: "Display the list of all available commands or a full description of the provided command.",
            examples: new string[]
            {
              "",
              "help"
            }
          ),
          execute: ExecuteHelpCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "command",
                description: "Nom de la commande.",
                defaultValue: null
              )
            }
          )
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
          description: new Command.Description(title: "Active/Désactive le mode marche."),
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
      {
        "hostile",
        new Command(
          name: "hostile",
          description: new Command.Description(title: "Rend hostile tous les joueurs de la zone non compris dans votre groupe actuel."),
          execute: ExecuteMakeAllPCInAreaHostileCommand
        )
      },
      {
        "publickey",
        new Command(
          name: "publickey",
          description: new Command.Description(title: "Affiche votre clef publique, utilisable sur les API du module."),
          execute: ExecuteGetPublicKeyCommand
        )
      },
      {
        "mute",
        new Command(
          name: "mute",
          description: new Command.Description(title: "Active/Désactive la réception des MP. Peut-être utilisé comme commande ciblée."),
          execute: ExecuteMutePMCommand
        )
      },
      {
        "listen",
        new Command(
          name: "listen",
          description: new Command.Description(title: "Commande DM : Active/Désactive le suivi de conversation."),
          execute: ExecuteListenCommand
        )
      },
      {
        "brp",
        new Command(
          name: "brp",
          description: new Command.Description(title: "Commande DM : Modifie le bonus roleplay de la cible."),
          execute: ExecuteSetRoleplayBonusCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "Bonus",
                description: "Bonus, doit être compris entre 0 et et 4.",
                defaultValue: ""
              )
            }
          )
        )
      },
      {
        "commend",
        new Command(
          name: "commend",
          description: new Command.Description(title: "Permet de recommander un joueur pour une augmentation de BRP. Disponible uniquement pour les joueurs de BRP 4."),
          execute: ExecuteCommendCommand
        )
      },
      {
        "jobs",
        new Command(
          name: "jobs",
          description: new Command.Description(title: "Permet d'afficher la liste et l'état des jobs en cours."),
          execute: ExecuteJobsCommand
        )
      },
      {
        "renamecreature",
        new Command(
          name: "renamecreature",
          description: new Command.Description(title: "Permet de modifier le nom de la créature ciblée, à condition qu'il s'agisse d'une de vos invocations."),
          execute: ExecuteRenameCreatureCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "Nom",
                description: "Le nouveau nom de la créature.",
                defaultValue: ""
              )
            }
          )
        )
      },
      {
        "test",
        new Command(
          name: "test",
          description: new Command.Description(title: "Permet des tester des trucs."),
          execute: ExecuteTestCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "skill",
                description: "Id du skill de test.",
                defaultValue: ""
              )
            }
          )
        )
      },
      {
        "menu",
        new Command(
          name: "menu",
          description: new Command.Description(
            title: "Affiche le menu permettant de configurer l'affichage du menu",
            examples: new string[]
            {
              "", "--reset"
            }
          ),
          execute: ExecuteMenuCommand,
          options: new Options(
            named: new Dictionary<string, Option>()
            {
              { "reset",
                new Option(
                  name: "reset",
                  description: "Reset la configuration du menu a sa config par défaut.",
                  defaultValue: false,
                  type: OptionTypes.Bool
                )
              },
            }
          )
        )
      },
      {
        "skills",
        new Command(
          name: "skills",
          description: new Command.Description(
            title: "Affiche le menu permettant de sélectionner de nouveaux skills à entrainer."
          ),
          execute: ExecuteSkillMenuCommand,
          options: new Options(
            named: new Dictionary<string, Option>()
            {
              { "config",
                new Option(
                  name: "config",
                  description: "Affiche la configuration du menu de skills.",
                  defaultValue: false,
                  type: OptionTypes.Bool
                )
              },
            }
          )
        )
      }
    };
  }
}
