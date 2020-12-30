using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static string ExecuteDisplayAnimationInfoCommand(SocketCommandContext context)
    {
      context.Channel.SendMessageAsync("Ce module se veut être un bac à sable sans grosse trame narrative dont la fin serait définie à l'avance. Certes, le monde autour de votre personnage vit, n'empêche que si vous n'allez pas chercher de vous même ce qu'il s'y passe, il ne vous arrivera pas grand chose et vous resterez tout bêtement à boire des coups à la taverne.\n\n" +
              "Les animations s'articuleront autour de plusieurs types :\n\n" +
              "    * Les projets PJs\n\n" +
              "    * Les investigations PJs\n\n" +
              "    * Les opportunités PNJs\n\n" +
              "    * Les animations d'ambiance\n\n" +
              "    * Les animations globales\n\n"
        );

      return "\n\n* PvP Sauvage\n\n" +
              "Il s'agit d'un type de PvP sans contrôle ou validation DM.\n" +
              "Dans ce cas toute résolution de combat technique utilise la même règle que pour un combat contre n'importe quel monstre : le ou les personnages à terre sont considérés comme K.O. ils seront ramenés à un dispensaire de soin et souffriront des malus de base.\n" +
              "Il est alors de bon ton pour les vaincus de ne pas retourner chercher des noises aux vainqueurs dans l'immédiat et de considérer que la remise sur pied prend une journée.\n\n";
    }
  }
}
