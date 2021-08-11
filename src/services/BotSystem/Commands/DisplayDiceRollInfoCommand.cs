using System.Threading.Tasks;
using Discord.Commands;

namespace BotSystem
{
    public static partial class BotCommand
    {
        public static async Task ExecuteDisplayDiceRollInfoCommand(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("Les dés sont là pour aider à simuler l'imprévisible dans certains situations.\n\n" +
              "Il ne faut cependant pas faire usage des dés dans n'importe quelle situation, car toutes ne sont pas propices à l'imprévu et à l'aléatoire.\n\n" +
              "Les dés peuvent être utilisés lorsqu'un personnage :\n\n" +
              "    - Est dans une situation critique, pressé par le temps(c'est le cas des combats)\n" +
              "    - Essaie de faire quelque chose d'extraordinaire (ou inhabituel)\n\n" +
              "Lorsqu'un personnage a tout son temps pour effectuer une action ou que cette action fait partie du savoir faire basique du personnage, il est contre-productif d'avoir recours à des jets de dés\n\n" +
              "Prenons l'exemple d'un pianiste professionnel qui joue une partition qu'il a longuement étudié et pour laquelle il a déjà effectué nombre de représentations. Dans des circonstances normales, il ne risque pas d'échouer : il maîtrise son instrument, c'est son métier, on ne lance donc pas de dés.\n\n" +
              "En revanche, s'il se trouvait dans une situation exceptionnelle telle que :\n\n" +
              "    - On le force à jouer avec un pistolet braqué sur la tempe\n" +
              "    - Il joue une partition qu'il n'a jamais vue ou étudiée auparavant\n" +
              "    - Il essaie de faire une interprétation exceptionnelle afin de conquérir l'audience\n\n" +
              "Là, il est susceptible d'échouer et effectuer un lancer de dés se justifie.\n\n" +
              "En revanche, il faut également tempérer l'échec : notre pianiste reste un professionnel, il ne va pas soudainement ne plus savoir jouer du piano parce qu'il a fait un échec critique. Sa compétence de pianiste n'est pas remise en cause. Mais il peut :\n\n" +
              "    - Craquer sous la pression, perdre ses moyens et fondre en larme(ça arrive quand on est menacé par une arme)\n" +
              "    - Jouer moins bien que d'habitude, mais seuls les autres professionnels de la musique s'en rendront compte : les amateurs n'y verront que du feu\n\n"
              );

            await context.Channel.SendMessageAsync("\n\nDisons qu'il existe une échelle de l'échec (et du succès) qui dépend du niveau de compétence du personnage.\n\n" +
              "Plus la compétence du personnage est élevée, moins les conséquences de son échec seront perceptibles et graves et plus ses réussites seront éclatantes.\n\n" +
              "L'échec critique d'un novice ne sera pas le même que celui d'un professionnel. Et idem pour les réussites critiques.\n\n");
        }
    }
}
