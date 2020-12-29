using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static string ExecuteDisplayLeitmotivInfoCommand(SocketCommandContext context)
    {
      context.Channel.SendMessageAsync("Le module est fondé autour de trois principes : \n\n" +
        "    - Le Role Play prime sur tout \n" +
        "    - La cohérence est fondamentale\n" +
        "    - Tout acte doit avoir des conséquences\n\n"
        );

      context.Channel.SendMessageAsync("\n\n1) Le Role Play prime sur tout\n\n" +
        "Une fois qu'un joueur se connecte au module, il accepte de mettre de côté tout ce qui ne concerne pas purement le rôle de son personnage pour se concentrer uniquement sur celui-ci.\n\n" +
        "Le HRP n'a pas sa place sur le module et doit être limité au strict minimum (permettre de résoudre des problèmes techniques liés aux limitations du jeu afin d'aider à contourner ces limitations d'une façon qui corresponde au rôle des personnages impliqués).\n\n" +
        "De cette façon, nous considèrons que les joueurs viennent principalement afin de pouvoir exprimer le rôle d'un personnage imaginaire. Bien jouer son rôle est le strict minimum attendu de la part de tous ceux qui viennent jouer. Les joueurs ne seront donc jamais récompensés simplement pour avoir joué ou participé à une animation.\n\n" +
        "En revanche, sera encouragée l'implication des joueurs dans la vie du module. Un joueur qui créé du contenu, créé lui-même des animations et embarque d'autres joueurs dans l'aventure afin de créer du lien, du conflit et tout ce qui génèrera du rp sera récompensé." +
        "Les récompenses en question seront purement rps et liés au à ce que le joueur aura aidé à mettre en place. Par exemple, un Heaumite qui organiserait une garde et une discipline (à laquelle plusieurs autres pjs participeraient) afin de faire respecter l'ordre pourrait se voir octroyer deux miliciens pnjs qui l'accompagneraient en tant que gardes du corps."
        );

      context.Channel.SendMessageAsync("\n\n2) La cohérence est fondamentale\n\n" +
        "Nous ne sommes pas pour un respect 'à la lettre' des règles, ainsi que du lore des ROs ou de D&D en général. Par exemple, modifier la race 'Orc' afin qu'ils ne soient plus de simples brutes stupides à massacrer à tour de bras, mais en faire une race de guerriers avec un code d'honneur strict, très proche de la nature serait envisageable.\n\n" +
        "En revanche, il est d'une importance cruciale de faire en sorte que le monde, son fonctionnement et les règles qui le régissent ne changent plus une fois qu'elles ont été révélées aux pjs.\n\n" +
        "De la même façon, il est attendu en retour de la part des joueurs qu'ils intègrent leurs personnages dans le monde en cohérence avec les règles de celui-ci, ce qui sera plus facile si elles sont stables et les mêmes pour tous."
        );

      context.Channel.SendMessageAsync("\n\n3) Tout acte doit avoir des conséquences\n\n" +
        "Lorsqu'un personnage entreprend une action, il est très important que celle-ci ait toujours des résultats (positifs ou négatifs). C'est ce qui permet de donner au monde un aspect vivant et un intérêt à s'y investir, ce qui peut se révéler à la fois être enrichissant ... et risqué. On ne se moque pas impunément des gens qui sont au pouvoir, par exemple !\n\n" +
        "Si personne ne réagit à rien et s'il n'y a pas de risque, alors certains joueurs ont tendance à mettre de côté toute prudence pour simplement tester les limites de ce qu'ils peuvent faire. Et une fois les limites testées ... tout le monde s'ennuie : tout étant possible, rien n'a véritablement de sens.\n\n" +
        "C'est une des raisons pour laquelle la mort-rp restera une option. Même si elle devait ne jamais être utilisée, il est important que chacun sache qu'elle fait partie des risques encourus, ne serait-ce que pour limiter les comportements suicidaires.\n\n"
        );

      context.Channel.SendMessageAsync("\n\nDernier point d'importance : ce module n'a pas pour but de raconter l'histoire d'une trame fixe et décidée à l'avance d'une campagne. \n\n" +
        "Le but de ce module est d'être un bac à sable pour de multiples histoires et interactions : tout d'abord, celles de vos personnages, de leur psychologie, leur façon de voir et d'agir sur le monde qui les entoure. Ensuite, l'histoire du monde en lui-même, qui est voué à évoluer en fonction de ce que ceux qui le font vivre (ses joueurs et le staff) souhaiteront raconter ensemble.\n\n"
        );

      return "Une bonne partie des animations seront à l'initiative des projets personnels des personnages qui souhaiteront modifier le monde dans lequel ils évoluent.\n\n";
    }
  }
}
