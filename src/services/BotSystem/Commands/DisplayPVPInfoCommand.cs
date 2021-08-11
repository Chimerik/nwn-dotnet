using System.Threading.Tasks;
using Discord.Commands;

namespace BotSystem
{
    public static partial class BotCommand
    {
        public static async Task ExecuteDisplayPVPInfoCommand(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("Le PvP consiste en n'importe quelle action conflictuelle entreprise par un personnage joueur contre un autre personnage joueur. Cette action peut aussi bien être le techniquement très simple 'Clic droit, attaquer' qu'une tentative plus complexe de détruire quelque chose mis en place par le personnage ciblé (sa faction, sa maison, son commerce, etc) et nécessitant généralement une intervention ou validation du staff.\n\n" +
                    "En cas de PvP, la principale règle qui s'applique est celle du fairplay.\n\n" +
                    "Nous essayons de construire un monde persistant et, dans l'idéal, un monde persistant cohérent. Cependant, il y a un certain nombre de considérations inhérentes à notre support (le jeu vidéo) à garder en tête afin de limiter les complications HRP.\n\n" +
                    "Certes, l'envie de votre personnage de couper la gorge de ses compagnons d'aventure rivaux pendant leur sommeil est parfaitement rp et cohérente, mais quelles sont les conséquences par rapport au gain de votre personnage ? \n\n" +
                    "Très probablement des joueurs dégoutés d'avoir perdu de façon absurde, et sans rien pouvoir y faire, un personnage dans lequel ils étaient investis émotionnellement. Joueurs qui quitteront alors le module. Module sur lequel vous vous retrouverez bientôt seul.\n\n" +
                    "A moins de circonstances particulières décrites dans !lamort, il convient d'éviter au maximum de rendre définitivement injouable un personnage. Mieux vaut infliger des conséquences temporaires dans l'espoir de résoudre le conflit plutôt que d'empêcher toute possibilité de jeu.\n\n"
              );

            await context.Channel.SendMessageAsync("\n\n* PvP Sauvage\n\n" +
                    "Il s'agit d'un type de PvP sans contrôle ou validation DM.\n" +
                    "Dans ce cas toute résolution de combat technique utilise la même règle que pour un combat contre n'importe quel monstre : le ou les personnages à terre sont considérés comme K.O. ils seront ramenés à un dispensaire de soin et souffriront des malus de base.\n" +
                    "Il est alors de bon ton pour les vaincus de ne pas retourner chercher des noises aux vainqueurs dans l'immédiat et de considérer que la remise sur pied prend une journée.\n\n");
        }
    }
}
