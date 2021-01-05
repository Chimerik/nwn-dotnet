using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDisplayAnimationInfoCommand(SocketCommandContext context)
    {
      await context.Channel.SendMessageAsync("Ce module se veut être un bac à sable sans grosse trame narrative dont la fin serait définie à l'avance. Certes, le monde autour de votre personnage vit, n'empêche que si vous n'allez pas chercher de vous même ce qu'il s'y passe, il ne vous arrivera pas grand chose et vous resterez tout bêtement à boire des coups à la taverne.\n\n" +
              "Les animations s'articuleront autour de plusieurs types :\n\n" +
              "    * Les projets PJs\n\n" +
              "    * Les investigations PJs\n\n" +
              "    * Les opportunités PNJs\n\n" +
              "    * Les animations d'ambiance\n\n" +
              "    * Les animations globales\n\n"
        );

      await context.Channel.SendMessageAsync("\n\nLes opportunités PNJs, animations d'ambiance et gobales sont purement au bon vouloir des animateurs. Il s'avère qu'il se passait quelque chose pile au bon moment et que votre personnage était pile au bon endroit pour participer.\n" +
              "Autant dire qu'il s'agit de quelque chose de peu fiable si vous souhaitez une activité régulière. L'idéal, pour cela, est plutôt de monter vos propres projets (ou de participer à ceux des autres). Les projets et enquêtes sont des formes d'animations dont vous et votre personnage (ou un autre joueur) est à l'initiative.\n" +
              "Il peut s'agir de quelque chose de tout simple comme 'je veux décrocher un job de serveur à la taverne du coin' tout comme de projets de plus grand envergure : 'Je veux devenir maître du monde'.\n" +
              "Ce qui compte, là-dedans, c'est comment vous allez vous y prendre en rp pour parvenir à votre objectif. Dans un premier temps, il conviendra de prendre contact avec le staff, directement en jeu ou via la commande !demandestaff qui pourra alors éventuellement vous donner des pistes qui vous permettront de structurer votre rp.\n" +
              "Bien entendu, aucune garantie n'est donnée quant au succès de votre entreprise. Peut-être échouerez-vous complètement, peut-être arriverez-vous à un endroit tout autre que celui que vous escomptiez. Quoiqu'il en soit, mieux vaut monter vos propres objectifs que de rester passif à la taverne à attendre qu'un PNJ bien disposé vous propose du boulot !\n");
    }
  }
}
