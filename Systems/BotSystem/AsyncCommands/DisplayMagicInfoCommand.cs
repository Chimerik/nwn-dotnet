using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDisplayMagicInfoCommand(SocketCommandContext context)
    {
      await context.Channel.SendMessageAsync("**La magie, qu'est ce que c'est ?**\n\n" +
        "Dans D&D et les ROs, c'est étonnamment plus simple que ce qu'on pourrait penser au premier abord.\n\n" +
        "La magie est une toile, administrée par la déesse de la magie: Mystra. Cette toile régissant l'ensemble des règles qui définissent la réalité, Mystra n'en a pas le contrôle absolu: seule la gérance lui a été déléguée.\n\n" +
        "Par exemple, avant la chute de l'empire de l'empire Néthérisse(passage de la 2 ème édition de D & D à la troisième), il était possible de lancer des sorts de niveau 12. C'est notamment ce qu'à fait Karsus lorsqu'il a tenté de voler une essence divine pour se l'accaparer(pas de bol, il a volé celle de la Mystra de l'époque et ça a tout fait péter). Les Mystra suivantes, ont décidé par prudence d'empêcher l'accès à des sorts d'une telle puissance et ont donc limité l'accès au niveau 12. Les sorts existent toujours, mais ils sont dissimulés dans la Toile et quiconque tenterait d'y accéder déclencherait un certain nombre d'alarmes posées par la déesse qui ne manquerait pas d'intervenir.\n\n" +
        "Bref, cet exemple pour illustrer que la Toile est une chose complexe, changeante, pleine de secrets et de mystères, que personne ne connait parfaitement et que personne ne contrôle totalement.\n\n" +
        "Pour bien vous représenter la Toile, imaginez - vous un métier un tisser. L'ensemble des fils du métier permet de tisser une Toile sous forme de tapisserie qui représenterait l'ordre et les règles du monde sous forme d'une infinité de petits motifs différents. Il est possible de manipuler ces fils afin de changer les motifs représentés sur la tapisserie. Lorsque l'on parvient à produire un motif précis, les règles de l'univers sont temporairement modifiées en reproduisant un effet particulier : c'est ce que l'on appelle un sort.\n\n"
        );

     await context.Channel.SendMessageAsync("\n\nOr non seulement manipuler ces fils est incroyablement complexe, mais encore faut-il trouver des motifs très précis qui permettent de produire des effets utiles et utilisables, ce qui demande des centaines, voire des milliers d'années d'études.\n\n" +
        "Vos personnages, au cours de leurs aventures, ne disposent généralement pas de suffisamment de temps pour explorer les mécanismes et les motifs de la Toile. Heureusement, d'illustres personnages s'y sont adonnés par le passé et ont laissé derrière eux une documentation rare et précieuse qui permet à ceux qui sont suffisamment futés d'apprendre à reproduire sur cette fameuse tapisserie cosmique des motifs utilisables.\n\n" +
        "L'étude de cette documentation et de ces motifs est déjà trop longue et difficile pour être accomplie lors d'une vie humaine normale, même pour quelqu'un qui ne ferait que ça de toute son existence sans jamais sortir de sa chambre d'études, alors imaginez pour quelqu'un dont l'occupation principale serait de partir à l'aventure.\n\n" +
        "Certaines créatures savent instinctivement comment reproduire ces motifs, sans véritablement savoir expliquer comment. C'est le cas des ensorceleurs. S'il fallait une comparaison moderne, disons que les ensorceleurs sont des artistes(peintres ou musiciens) qui réalisent des œuvres, tandis que les mages sont des mathématiciens qui essaient de comprendre les règles qui ont été suivies pour obtenir ces œuvres.\n\n" +
        "Ce qu'il faut retenir : lorsqu'on manipule les fils de la Toile, on modifie de façon brutale des motifs pré - existants sur la dite Toile pour les réarranger d'une manière différente qui permette de reproduire les effets de sorts souhaités. Il n'y a pas de 'flux d'énergie brute' que l'on manipule pour en faire ce qu'on veut, mais bien des dessins précis que l'on esquisse pour produire un effet précis et nul autre.\n\n"
        );

      await context.Channel.SendMessageAsync("\n\nPour être tout à fait précis, quelqu'un qui manipulerait le métier à tisser d'où les fils de la tapisserie sont originaires, pourrait créer des 'flux' de fils et produire des motifs qui ne nécessiteraient pas de déformer l'existant. C'est ce que fait la Haute Magie elfique, qui nécessite 500 ans d'entrainement avant de commencer à comprendre les bases.\n\n" +
        "Autant vous dire que c'est un type de magie uniquement utilisable dans le cadre du 'lore' et d'une intrigue, afin d'ajouter des éléments de mystère, mais que des aventuriers ne pourront jamais apprendre ou maitriser.\n\n");
    }
  }
}
