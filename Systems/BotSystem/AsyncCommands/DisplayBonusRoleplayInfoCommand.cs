using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDisplayBonusRoleplayInfoCommand(SocketCommandContext context)
    {
      await context.Channel.SendMessageAsync("**Le bonus Role Play**\n\n" +
        "Le Bonus Role Play(aussi connu sous le nom de BRP) est un nombre entre 0 et 4 qui indique la façon dont votre implication Role Play et votre comportement en jeu est perçue par les DMs. Ci-dessous, un guide général au sujet de ces bonus :\n\n" +
        "0 : Absence de Role Play, non respect des règles ou abus du système.\n" +
        "1 : Les nouveaux personnages dont le background n'a pas encore été validé, ainsi que tous ceux qui font le strict minimum.\n" +
        "2 : Standard. Reste en permanence fidèle au RP de son personnage. Participe au fun des autres joueurs. A un personnage avec une véritable personnalité et qui fasse réellement partie du monde.\n" +
        "3 : Un joueur qui aide à organiser de mini-événements et animations et dont les actions ont des conséquences rp significatives à l'échelle de la vie du module\n" +
        "4 : Ceux qui font bouger le module et le rp des autres. Qui créent deshistoires cohérentes avec la volonté d'apporter des changements dont les conséquences influenceront les autres joueurs au travers des tensions, conflits générés, aidant ainsi les autres à développer leur rp et l'histoire du monde.\n\n"
        );

      await context.Channel.SendMessageAsync("\n\nLa vitesse globale de progression de l'entrainement de capacité et de craft dépend de ces niveaux :\n\n" +
        "    0 : 0 %\n" +
        "    1 : 90 %\n" +
        "    2 : 100 %\n" +
        "    3 : 110 %\n" +
        "    4 : 120 %\n" +
        "Le principe du Bonus de Role Play est d'être dynamique. Il augmentera ou diminuera selon votre expérience sur le module tandis que les joueurs sont observés en différentes circonstances, selon le niveau de roleplay dont ils tâchent de faire preuve. Essayez de ne pas prendre personnellement une diminution : le dm à l'origine de celle - ci a peut-être une suggestion ou un commentaire à faire sur la raison de cette baisse.\n\n" +
        "Violer les règles aura pour conséquences une chute rapide, probablement à 0. N'ayez pas peur, brave aventurier, il sera toujours possible de l'augmenter de nouveau en faisant preuve de bon rp et de respect des règles. Souvenez - vous simplement que le Bonus de RP est juste ... un bonus. C'est notre façon de récompenser les joueurs qui aident à faire du module un monde cohérent dans lequel tout le monde s'amuse.\n\n" +
         "Si vous connaissez quelqu'un dont le rp est excellent, n'hésitez pas à le nommer pour une augmentation de bonus !\n\n" +
        "N'oubliez pas que les BRP 1-2 ont pour objet votre roleplay alors que les BRP 3-4 concernent l'influence de votre personnage sur le monde et la façon dont il inspire les autres joueurs.\n\n" +
        "Afin de s'assurer de la visibilité du rp de chacun, nous avons ajouté une commande chat '!commend', utilisable par les joueurs avec un BRP de 4. Cela permet à ces joueurs de recommander ceux dont ils estiment le rp. Cette commande à deux effets :\n" +
        "    * Premièrement, si le joueur recommandé se trouve à 1 BRP, il obtient immédiatement une augmentation à 2 BRP.\n" +
        "    * Deuxièmement, la recommandation est diffusée aux DMs, afin que ceux-ci puissent plus rapidement concentrer leur évaluations sur ceux dont le rp a déjà été repéré.\n\n" +    
        "Le but est à la fois de prendre en compte les nouveaux joueurs plus rapidement et aussi de donner davantage de temps aux DMs de s'occuper des passages BRP de 2 à 3. \n\n");
    }
  }
}
