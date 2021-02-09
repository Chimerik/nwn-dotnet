using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
    public static partial class BotSystem
    {
        public static async Task ExecuteDisplayEvilInfoCommand(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("Le rp evil a le potentiel d'être un énorme moteur d'animation entre joueurs sur un module, permettant d'alléger nettement la charge qui pèse sur les dms en insérant des relations conflictuels entre joueurs qui alors n'ont même plus besoin d'être directement animés.\n\n" +
              "Cependant, le rp evil ne doit jamais être utilisé comme la justification d'un simple désir de faire chier d'autres joueurs.Il ne doit jamais réduire les options de rp disponibles, mais toujours ouvrir de nouvelles options et de nouveau conflits.Le but est de jouer quelqu'un de maléfique, pas de jouer un gros con.\n\n" +
              "Faire un perso evil dans le but de le faire xp au maximum pour ensuite établir une domination sur les autres joueurs est la pire manière d'appréhender le concept de mal.\n\n" +
              "Ruiner le plaisir de jeu d'un autre joueur 'parce que c'est ce que ferait mon perso: après tout, il est chaotique evil, il aime pk tout le monde à vue!' n'est pas une réponse acceptable.\n\n" +
              "Un bon rp evil doit être subtil, proposer des alternatives et permettre de construire quelque chose.\n" +
              "Quelques exemples d'archétypes de personnage evil afin de donner des pistes d'idées (à noter que ce ne sont que des pistes et des possibilités d'interprétation parmi des myriades d'autres) :\n\n" +
              "    *Loyal Mauvais: La mafia / pègre, construire un réseau qui fonctionne dans l'illégalité avec ses propres lois et ses propres codes, à côté d'un autre système de pouvoir existant\n" +
              "    * Neutre Mauvais: Peu importe les méthodes employées tant que les résultats sont là.Cet alignement est souvent caractéristique de l'égoïsme\n" +
              "    * Chaotique Mauvais: La seule loi qui vaille est celle du plus fort, les faibles doivent respect et obéissance à tout individu capables de se montrer plus fort qu'eux.\n\n" +
              "Quoiqu'il en soit, nous serons vigilants à tout tentative d'utiliser le rp evil pour détruire du rp plutôt que d'en créer.\n\n"
              );

            await context.Channel.SendMessageAsync("\n\nLes joueurs intéressés à l'idée de s'investir dans un rp constructif evil seront aidés par le staff et des facilités leur seront accordées pour réaliser leurs actions. Cependant, la contrepartie de cette facilité est qu'il faut accepter de perdre : la plupart du temps, les plans des evils finiront par être mis à bas par les 'goods' (voire neutres, voire d'autres evils qui auraient d'autres objectifs).\n\n" +
            "Mais est-ce bien grave ? L'échec de tel plan était peut-être prévu. Ne faisait-il pas en réalité partie d'un plan encore plus vaste ?\n" +
            "D'autant que les Erylies sont des terres de colonisation et d'opportunités. Aucune puissante structure de Loi ou de Bien n'est encore en place, ce qui devrait laisser de le champ libre aux opportunistes.\n\n");
        }
    }
}
