using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
    public static partial class BotSystem
    {
        public static async Task ExecuteDisplayRoleplaySpellsInfoCommand(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("\n\n**Gestion des sorts rp :**\n\n" +
              "Les sorts rp correspondent aux sorts absents de NwN, mais présents dans les manuels de D&D (ou dont le fonctionnement dans NwN diffère de celui de D&D), ou n'importe quel effet qu'un joueur aimerait reproduire par magie.\n\n" +
              "Pour faire simple : l'utilisation de tels sorts n'est pas autorisée sur les Larmes des Erylies. Tout sort utilisé par votre personnage doit consommer un emplacement de sort mémorisé dans votre grimoire. Si votre sort n'est pas mémorisé, c'est que vous ne pouvez pas l'utiliser.\n\n" +
              "En revanche, tout arcaniste est susceptible d'effectuer des recherches afin d'inventer des sorts encore non disponibles dans le jeu. Voici la procédure à suivre :\n" +
              "    * Avoir obtenu l'accord de principe du staff en fournissant les caractéristiques du sort (niveau, effet souhaité, école, etc)\n" +
              "    * Avoir un rp de spécialiste dans le domaine concerné : seul un personnage disposant d'un rp d'invocateur très développé et spécialisé en invocation pourra éventuellement apprendre et faire usage d'un sort de téléportation\n" +
              "    * Avoir effectué des recherches rp sur le sujet et impliqué d'autres personnages dans une mini-animation liée à cette étude rp. Le but étant de générer autant d'interactions entre joueurs que possible.\n" +
              "    * Une fois les conditions précédentes réunies et la validation finale de l'équipe DM obtenue, le sort sera ajouté au jeu (ce qui peut prendre du temps). Un parchemin vous sera alors donné, étudiable via le système de leveling comme n'importe quel autre sort.\n" +
              "    * Vous seul serez alors possesseur de ce sort. Libre à vous, cependant, d'en fabriquer des parchemins via l'artisanat afin de permettre à d'autres de l'apprendre à leur tour.\n\n");
        }
    }
}
