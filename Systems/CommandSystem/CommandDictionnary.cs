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
      /*{
        "frostattack",
        new Command(
          name: "frostattack",
          description: new Command.Description(title: "Permet aux lanceurs de sorts d'utiliser rayon de giver comme attaque par défaut."),
          execute: ExecuteFrostAttackCommand
        )
      },*/
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
        "medic",
        new Command(
          name: "medic",
          description: new Command.Description(title: "Dissipe tout effet de maladie sur vous pour faciliter l'alpha"),
          execute: ExecuteDispelDiseaseCommand
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
        "refill",
        new Command(
          name: "refill",
          description: new Command.Description(title: "Lié à reboot et aux tests"),
          execute: ExecuteRefillCommand
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
        "recycle",
        new Command(
          name: "recycle",
          description: new Command.Description(title: "Permet de tester le recyclage."),
          execute: ExecuteRecycleCommand
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
        "contrat",
        new Command(
          name: "contrat",
          description: new Command.Description(title: "Affiche le menu de rédaction/consultation de contrats privés d'échangé de ressources."),
          execute: ExecuteContractMenuCommand
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
        "sit",
        new Command(
          name: "sit",
          description: new Command.Description(title: "Permet de s'asseoir."),
          execute: ExecuteSitCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "hauteur",
                description: "Hauteur",
                defaultValue: "5"
              )
            },
            named: new Dictionary<string, Option>()
            {
              { "down",
                new Option(
                  name: "down",
                  description: "Utilise l'animation assis au sol, jambes croisées, plutôt que l'animation assis sur une chaise.",
                  defaultValue: false,
                  type: OptionTypes.Bool
                )
              },
            }
          )
        )
      },
      {
        "rename",
        new Command(
          name: "rename",
          description: new Command.Description(title: "Commande DM - Permet de modifier le nom de la cible."),
          execute: ExecuteRenameCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "Nom",
                description: "Le nouveau nom de la cible.",
                defaultValue: ""
              )
            }
          )
        )
      },
      {
        "tag",
        new Command(
          name: "tag",
          description: new Command.Description(title: "Commande DM - Permet de modifier le tag de la cible."),
          execute: ExecuteTagCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "Tag",
                description: "Le nouveau tag de la cible.",
                defaultValue: ""
              )
            }
          )
        )
      },
      {
        "description",
        new Command(
          name: "description",
          description: new Command.Description(title: "Modifie la description de votre personnage à partir du nom de la description enregristré via Discord."),
          execute: ExecuteLoadDescriptionCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "Nom",
                description: "Le nom de la description enregistrée.",
                defaultValue: ""
              )
            }
          )
        )
      },
      {
        "saveapparence",
        new Command(
          name: "saveapparence",
          description: new Command.Description(title: "Permet de sauvegarder l'apparence de l'objet sélectionné."),
          execute: ExecuteSaveAppearanceCommand
        )
      },
      {
        "restoreapparence",
        new Command(
          name: "restoreapparence",
          description: new Command.Description(title: "Permet d'appliquer une apparence d'objet sauvegardée à l'object sélectionné."),
          execute: ExecuteLoadAppearanceCommand
        )
      },
      {
        "resetpos",
        new Command(
          name: "resetpos",
          description: new Command.Description(title: "Réinitialise la position d'affichage du personnage à sa position réelle."),
          execute: ExecuteResetPositionCommand
        )
      },
      {
        "stuck",
        new Command(
          name: "stuck",
          description: new Command.Description(title: "Permet de décoincer un personnage bloqué dans le décor."),
          execute: ExecuteStuckCommand
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
        "supprimer",
        new Command(
          name: "supprimer",
          description: new Command.Description(title: "Attention, cette commande supprime définitivement le personnage avec lequel vous êtes actuellement connecté."),
          execute: ExecuteDeleteCharacterCommand
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
      },
      {
        "stocks",
        new Command(
          name: "stocks",
          description: new Command.Description(
            title: "Affiche votre stock personnel de matières premières"
          ),
          execute: ExecuteStocksMenuCommand
        )
      },
      {
        "test_arena",
        new Command(
          name: "test_arena",
          description: new Command.Description(title: "test pour le systeme d'arene"),
          execute: ExecuteTestArenaCommand
        )
      },
      {
        "pnj",
        new Command(
          name: "pnj",
          description: new Command.Description(title: "Commande dm : permet d'ouvrir le menu de modification de pnjs"),
          execute: ExecutePNJFactoryCommand
        )
      }
    };
    }
}
