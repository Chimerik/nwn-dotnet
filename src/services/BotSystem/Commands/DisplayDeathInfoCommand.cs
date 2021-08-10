using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
    public static partial class BotSystem
    {
        public static async Task ExecuteDisplayDeathInfoCommand(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("1) La mort technique\n\n" +
              "Définition: Il s'agit de la mort d'un personnage lors d'une situation qui ne 'raconte rien'. Elle reste rp, puisque tout doit être autant rp que possible, mais étant donné que notre support a de nombreuses faiblesses (afk du joueur, lag, spawn et IA des mobs non contrôlés, difficulté mal ajustée, bugs, etc), ses conséquences doivent être limitées afin de ne pas rendre le jeu injouable.\n\n" +
              "Le 'mort' est alors considéré comme ayant été ramené en vie au dispensaire, remis sur pied et soigné.\n\n" +
              "Conséquences :\n\n" +
              "    - Le joueur victime d'une mort technique laisse sur place la totalité des ressources de craft contenues dans son inventaire, la totalité de l'or qu'il avait sur lui et les patrons de craft qu'il avait eu le malheur d'emmener\n" +
              "    - La victime paye une somme en or afin de rembourser les soins qui ont permis de le remettre sur pied. Cette somme est directement prélevée sur son compte en banque. S'il ne dispose pas de la somme requise, il écope d'une dette qui augmentera selon un taux d'usurier avec le temps.\n" +
              "    - La victime acquiert un malus aléatoire. Il est possible de s'en débarrasser avec le temps via le système de leveling.\n\n"
              );

            await context.Channel.SendMessageAsync("\n\n 2) La mort dite 'rp'\n\n" +
              "Il s'agit là d'une disparition longue, voire définitive du personnage victime validée par le joueur concerné ou par le staff dm.\n\n" +
              "La disparition peut n'être que longue dans certains cas où il serait nécessaire que d'autres joueurs entreprennent des recherches pour trouver le corps, par exemple.\n\n" +
              "Les sorts de rappel à la vie et de résurrection du jeu de base permettent seulement de soigner la condition KO et ne font pas passer de trépas à vie. Des sorts de résurrection véritables pourront être recherchés par les arcanistes, mais ceux-ci seront rares et couteux (voir !sortsrp). \n\n" +
              "La disparition définitive, doit rester possible afin de limiter les comportements suicidaires de joueurs qui en viendraient à penser que rien ne peut arriver à leur personnage.\n\n" +
              "Elle doit cependant rester rare: un joueur doit être averti du risque définitif encouru par son personnage et doit avoir la possibilité d'emprunter une autre voie moins dangereuse.\n\n" +
              "En cas de conflits entre joueurs, fairplay sera le maître mot.  La plupart des combats devront être résolus par d'autres conséquences que la mort définitive (disparition temporaire, malus, réputation entachée, droits retirés, ressources pillées, etc).\n\n" +
              "En cas de nécessité d'infliger une disparition définitive dans un conflit entre joueurs, la règle de prévention s'applique également : les joueurs impliqués doivent avoir été avertis du risque et doivent avoir une voie d'échappatoire.\n\n" +
              "S'ils choisissent cette échappatoire moins risquée, ils seront alors considérés comme perdants du conflit et devront se soumettre à certaines conditions et exigences de la part de ceux l'ayant emporté.\n\n" +
              "Dans le cas où ces conditions et exigences ne seraient pas remplies, une disparition définitive pourrait être infligée.\n\n"
              );

            await context.Channel.SendMessageAsync("Exemple : un personnage joueur insulte publiquement un autre personnage (pas forcément joueur) dépositaire de l'autorité. L'autorité dépêche la garde qui arrête le personnage joueur. L'autorité étant un salopard (ACAB) et détestant le personnage en question, elle n'exigera pas son exécution (ce qui serait pourtant cohérent), mais pourra demander des sanctions injustes.\n\n" +
              "Le personnage décide plutôt d'essayer de s'échapper et se retrouve à se battre contre l'autorité. Dans le cas où l'autorité l'emporte, celle-ci pourra alors exiger que le personnage soit désormais forcé de faire preuve de respect en public envers elle. Si le personnage continue par la suite ses injures, en cas de nouveau conflit et sur validation du staff, une exécution véritable pourrait être envisagée.\n\n" +
              "Dans le cas où le personnage l'emporterait, plutôt que de l'assassiner, l'autorité serait contrainte de devoir supporter les injures publiques en se contentant de grincer des dents.\n\n" +
              "Le but étant de faire en sorte que les actes aient des conséquences. Tout en proposant des solutions alternatives aux joueurs. Si même les solutions alternatives ne sont pas respectées, alors on applique la conséquence définitive (tout en respectant la cohérence des scènes jouées et de l'univers de jeu).\n\n"
              );

            await context.Channel.SendMessageAsync("\n\nDeux points importants à garder en tête: \n" +
              "    - Les risques encourus dépendent du type d'action entrepris. Tenter de voler de l'or à un PNJ implique de risquer de se faire tabasser. Tenter de tuer un PNJ impliquera toujours de risquer d'être soi-même tué.\n" +
              "    - La mort ne doit jamais être gratuite. Elle doit pouvoir permettre au joueur qui perd un personnage de raconter une histoire.\n\n");
        }
    }
}
