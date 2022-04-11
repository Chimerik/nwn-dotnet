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
            title: "Affiche la liste de toutes les commandes disponibles.",
            examples: new string[]
            {
              ""
            }
          ),
          execute: ExecuteHelpCommand
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
        "kick",
        new Command(
          name: "kick",
          description: new Command.Description(title: "Permet de kick un joueur hors du serveur."),
          execute: ExecuteKickCommand
        )
      },
      {
        "tp",
        new Command(
          name: "t^p",
          description: new Command.Description(title: "Commande de téléportation DM."),
          execute: ExecuteDMTeleportationCommand
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
        "mute",
        new Command(
          name: "mute",
          description: new Command.Description(title: "Active/Désactive la réception des MP. Peut-être utilisé comme commande ciblée."),
          execute: ExecuteMutePMCommand
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
        "vfx",
        new Command(
          name: "vfx",
          description: new Command.Description(title: "Permet d'essayer les effets visuels."),
          execute: ExecuteVFXCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "vfx",
                description: "Id du vfx à tester.",
                defaultValue: 0
              )
            }
          )
        )
      },
      {
        "suivre",
        new Command(
          name: "suivre",
          description: new Command.Description(title: "Suit automatiquement le personnage ciblé."),
          execute: ExecuteFollowCommand
        )
      },
      {
        "dm",
        new Command(
          name: "dm",
          description: new Command.Description(
            title: "Affiche le menu dm."
          ),
          execute: ExecuteDMMenuCommand
        )
      }
    };
    }
}
