using System.Threading.Tasks;
using Discord.Commands;

namespace BotSystem
{
    public static partial class BotCommand
    {
        public static async Task ExecuteDisplayUniverseInfoCommand(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("L'univers du module a une connexion forte avec les Royaumes Oubliés afin de permettre aux joueurs de disposer d'une base commune classique et bien connue sur laquelle établir leur background initial.\n\n" +
              "Cependant, les RO nécessitant une adaptation à NwN et au système 'jeu vidéo', nous avons préféré établir la base même du module dans un univers différent de façon à pouvoir nous affranchir du lore de Féerune lorsque celui-ci n'est pas applicable à ce que nous souhaitons faire/raconter, tout en continuant à y puiser les éléments susceptibles de nous intéresser.\n" +
              "L'essentiel de l'intrigue se déroule dans un archipel d'îles inconnues, ce qui présente plusieurs avantages : \n\n" +
              "- Cloisonner le terrain explorable au mapping déjà réalisé\n" +
              "- Agrandir le terrain disponible au fur et à mesure en rajoutant des îles plus ou moins éloignées avec des biomes et des climats uniques lorsque les maps sont prêtes\n" +
              "- Permettre l'existence rp de civilisations entières sans avoir à réellement créer les maps correspondantes. Un empire entier pourrait exister dans des îles non disponibles sur le module, mais pourrait exercer son influence sur les îles explorables, par exemple.\n\n" +
              "Ce serait notamment le cas de Féerune, dont les maps n'existeront jamais sur le module, mais qui pourra y exercer son influence.\n\n"
              );

            await context.Channel.SendMessageAsync("\n\nL'arrivée des pjs sur le module se fait de la façon suivante :\n\n" +
              "Un archipel d'îles mystérieuses et inconnues a été découvert au beau milieu d'un océan de Féerune. Plusieurs empires concurrents ont alors décidé d'y envoyer troupes et pionniers afin d'être les premiers à prendre possession de ces nouvelles terres et de leurs richesses.\n\n" +
              "Problème, cet archipel se trouve au beau milieu d'une sorte de 'Triangle des Bermudes' local : la navigation est difficile et dangereuse. Pour chacun de ces empires, la communication a finalement été rompue avec les troupes dépêchées sur place.\n\n" +
              "Un peu échaudés par cet échec et peu désireux d'engager de nouvelles dépenses pharaoniques, les empires font mander des aventuriers afin d'affréter des navires qui auraient pour mission de rétablir la communication entre eux et les pionniers. La récompense annoncée est un dominion et une baronnie sur des îles de l'archipel.\n\n"
              );

            await context.Channel.SendMessageAsync("Nos pjs sont donc ceux qui, d'une manière ou d'une autre, ont répondu à cet appel.\n");
        }
    }
}
