using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDisplayAlignementInfoCommand(SocketCommandContext context)
    {
      await context.Channel.SendMessageAsync("Sur les Larmes, l'alignement n'est pas une mesure ponctuelle de l'état d'esprit d'un personnage à l'instant T. Chaque action du personnage ne provoque pas un gain de points dans telle ou telle direction.\n" +
        "Tout d'abord, parce que bien plus que l'action, c'est l'intention avec laquelle elle est faite qui compte, ensuite parce qu'un personnage qui rentrerait entièrement dans une case d'alignement manquerait cruellement de profondeur et serait bien trop simpliste.\n\n" +
        "L'alignement est un guide général permettant de cadrer la façon dont un personnage perçoit le monde et comment il est le plus susceptible de réagir face à une situation donnée.\n\n" +
        "Pour changer d'alignement, il faut une modification profonde de la psychologie du personnage qui l'amènerait à bousculer entièrement la façon dont il perçoit le monde.\n\n" +
        "Donner 10 pièces à un mendiant ne vous fera pas gagner de point vers l'alignement bon.\n" +
        "En revanche, un personnage qui accumulait de la richesse, considérant l'avoir acquise par sa propre force et son mérite (donc qu'aucun autre que lui n'y a le droit), et qui, au gréé de ses aventures, réaliserait que la fortune n'a pas de saveur si elle n'est pas partagée car la solitude deviendrait pour lui un bien pire mal que la pauvreté, serait tout à fait susceptible de changer d'alignement, car c'est là un profond bouleversement de sa façon de voir le monde.\n\n"
        );

      await context.Channel.SendMessageAsync("L'autre aspect qui pose généralement problème avec le système d'alignement est leur définition.\n\n" +
        "Qu'est ce qu'être Loyal ? Chaotique ? Bon ? Mauvais ? Neutre ?\n\n" +  
        "Concevoir ce système comme un guide général plutôt que comme un ensemble de restrictions précises permet de mieux concevoir ce rapport entre alignements.Voici les définitions que nous vous proposons:\n\n" +
        "**Loyal :** il ne s'agit en aucun cas de la 'loyauté' du personnage. Il s'agit du rapport que le personnage a vis à vis de la loi. Lorsqu'il se trouve face à une règle qui lui complique la tâche ou qui ne va pas dans son sens, le personnage d'alignement Loyal aura une très forte tendance à respecter la règle (et la loi) à la lettre, même si ça lui pose problème.\n" +
        "La vision du monde d'un personnage Loyal tourne autour de : 'Le système actuel (la Loi) est en place pour une bonne raison, mieux vaut donc préserver le statu quo et corriger de l'intérieur, au cas par cas, les règles qui ne fonctionnent pas, plutôt que de provoquer un bouleversement qui ne manquerait pas d'être dangereux. C'est aux gens de s'adapter au système'.\n\n" +
        "**Chaotique :** A l'inverse de Loyal, l'alignement Chaotique aura une très forte tendance à considérer que c'est au système de s'adapter à chaque situation. Pour lui, le monde est trop complexe pour être régit par des règles fixes, ce qui signifie pas qu'il ne doit pas y avoir de règles, mais que chaque situation étant différente et unique, elle doit être jugée de façon différente et unique de façon à pouvoir apporter la meilleure réponse possible et les changements qui s'imposent.\n\n"
        );

      await context.Channel.SendMessageAsync("Voici un exemple de la vie courante qui permet d'illustrer ce type de nuances :\n\n" +
        "Un personnage cherche à traverser la route.\n\n" +
        "    * Le Loyal se dirige vers le passage piéton le plus proche, attend que le feu piéton soit au vert, puis traverse.\n" +
        "    * Le Neutre se dirige vers le passage piéton le plus proche, jette un coup d'œil à la route, s'il n'y a aucun véhicule, il traverse même au rouge, sinon il attend le vert.\n" +
        "    * Le Chaotique jette un coup d'œil à la route, s'il estime que le prochain véhicule est suffisamment loin, il traverse en dehors du passage piéton.\n" +
        "Après, en suivant cet exemple, il y a plusieurs degrés de 'Loyal' et de 'Chaotique'.\n" +
        "Un Loyal extrême irait faire la moral à un Neutre qui traverse au feu piéton rouge, même s'il n'y a aucun véhicule sur la route.\n" +
        "Un Chaotique extrême traverserait hors du passage piéton sans même regarder, forçant les véhicules en présence à ralentir et s'arrêter.\n\n" +
        "C'est ce qui est chouette lorsqu'on sort de l'interprétation rigide des alignements et qu'on les considère comme un guide général: il y a plein de place pour la nuance !\n"
        );

      await context.Channel.SendMessageAsync("On entre maintenant dans des considérations philosophiques plus complexes : la notion de **Bien** et de **Mal**. D&D (et les RO) ont une vision particulièrement manichéenne de cet aspect et souvent pas très cohérente : il y a des dieux bons et des dieux mauvais, des races bonnes et des races mauvaises.\n\n" +
        "Si une telle façon de penser fonctionnait avec l'état d'esprit des années 50 - 80, elle est difficilement entendable en 2020 et même Wizards of the Coast a commencé à revenir là-dessus en annonçant différents changements à venir quant à la nature maléfique de ses races, notamment concernant les drows qui, de purement maléfiques, deviendraient des esclaves d'une divinité et d'une société malfaisante, sans que ce 'mal' ne soit directement inscrit dans leurs gènes.\n\n" +
        "En effet, philosophiquement parlant, le Bien et le Mal n'ont pas de sens s'ils ne sont pas vus et jugés par le prisme d'une société et d'un système précis. Dans notre cas, nous avons une forte tendance à juger selon le prisme d'une société occidentale, catholique et capitaliste, mais bien d'autres sociétés seraient envisageables surtout dans un monde de fantaisie où Dieux, diables et démons sont une réalité concrète.\n\n" +
        "C'est pourquoi, dans notre cas, nous simplifieront le Bien et le Mal aux notions suivantes :\n\n" +
        "**Mauvais :** A une très forte tendance à considérer que non seulement son intérêt propre prime sur celui des autres, mais surtout que l'impact de ses actions sur le bien-être des autres est complètement négligeable à partir du moment où son bénéfice personnel le justifie.\n\n" +
        "**Bon :** Inversement, aura une forte tendance à être altruiste et à se mettre en danger pour défendre l'intérêt général, même au détriment de son intérêt personnel.\n\n" +
        "Ainsi donc, un personnage mauvais n'ira pas nécessairement nuire aux autres, juste 'parce qu'il le peut'. Il est tout à fait capable de s'insérer dans une société et d'y participer de façon constructive. En revanche, il se fera passer avant les autres quelles que soient les conséquences pour les autres.\n\n"
        );

      await context.Channel.SendMessageAsync("De la même façon, un personnage bon peut parfaitement avoir à recours à des actions 'mauvaises' à partir du moment où il considère que ces actions bénéficient à l'intérêt général.\n\n" +
        "* Exemple : un Chaotique Bon ne verra aucun problème à exécuter sommairement et sans procès un prêtre de Cyric, considérant que laisser celui-ci s'installer davantage dans la société aurait des conséquences négatives évidentes pour la population.\n" +
        "Un Loyal Bon, en revanche, aura davantage tendance à vouloir organiser un procès selon les règles établies avant que le dit prêtre ne soit exécuté. A moins qu'une loi n'autorise expressément l'exécution à vue des prêtres de Cyric.\n\n" +
        "* Exemple bis: un Loyal Mauvais pourrait parfaitement décider de militer contre l'esclavage d'une population, à partir du moment où il considère que mettre fin à cet esclavagisme permettrait aux anciens esclaves de gagner un salaire et donc d'acheter les dangereuses drogues qu'il vend, augmentant son marché et sa part de profits.Il ne deviendrait pas pour autant 'Bon' en mettant fin à l'esclavage.\n\n"
        );

      await context.Channel.SendMessageAsync("Enfin, l'alignement gardé pour la fin :\n\n" +
        "**Neutre :** Cet alignement est plus complexe à gérer que les autres, car il est le moins défini par un cadre. Or, le système d'alignement étant là pour donner un cadre général à une personnalité, comment définir quelqu'un qui n'est ni dans un cadre précis, ni dans l'autre ?\n\n" +
        "Notre réponse est: l'alignement neutre n'ira pas faire d'importants efforts spécialement pour faire respecter la Loi, ni pour faire expressément un profit personnel au détriment des autres. C'est simplement que les choses s'étant passées d'une certaine façon, il a simplement suivi le courant qui a débouché sur la conclusion actuelle.\n\n" +
        "Par exemple, un garde Loyal Neutre n'aura pas de forte inimitié envers un suivant de Mask à partir du moment où les agissements du suivant n'impactent pas le garde, sa famille et ses amis. C'est juste que ... être garde, c'est son boulot et le suivant de Mask s'est fait chopper. Si le garde avait été boulanger, que le suivant avait été plus discret, le garde n'aurait pas eu besoin de courser le suivant, de le rouer de coups et de l'enfermer en geôles. Dans une autre situation, ils auraient même pu être potes ! Mais, hey, c'est la vie et ça s'est passé comme ça, que voulez-vous ?\n\n" +
        "L'alignement **Neutre Strict** reprend cette logique, parfois de façon un peu plus extrême, notamment pour les druides.\n\n" +
        "* Exemple : Un barrage a été installé en amont et bloque le fleuve, permettant à un village humain de s'installer dans une zone qui se trouvait précédemment sous les flots. Cependant, ce barrage bouleverse profondément la faune et la flore en contrebas qui se met à dépérir, faute d'eau auparavant abondante.Le druide Neutre Strict débarque et ne verra aucun problème à faire sauter le barrage, noyant hommes, femmes et enfants en contrebas.C'est juste que ... les choses se sont passées ainsi, tant pis : si le village avait été construit ailleurs, il n'aurait pas été englouti!\n"
        );

      await context.Channel.SendMessageAsync("(Après, il y a des nuances : un druide neutre bon aurait fait tout son possible pour que le village déménage avant de faire sauter le barrage. Un druide neutre strict avertirait le village de son intention et leur laisserait un peu de temps pour déménager, un druide neutre mauvais n'en aurait rien à cirer et ferait sauter le barrage, peu importe ce qui se trouve en dessous)\n\n"
        );

      await context.Channel.SendMessageAsync("\n\n**Résumé TLDR :**\n\n" +
        "    * Loyal = Honneur(respect des engagements, de la parole donnée et des contrats), tradition, stabilité\n" +
        "    * Chaotique = Changement, adaptation, progrès\n" +
        "    (Neutre = Je respecte quand ça arrange la cause que je supporte, mais j'ai tout de même principalement tendance à ranger du côté du système établi qui a fait ses preuves)\n" +
        "    * Bon = Altruisme : je me met en danger pour défendre des causes d'intérêt général, même si c'est au détriment de mes intérêts personnels et même si c'est une cause perdue\n" +
        "    * Mauvais = Mettre en danger les autres pour mon intérêt personnel ne me pose aucun problème\n" +
        "    * (Neutre = Je n'irai pas défendre une cause qui ne me concerne pas directement, mais je n'hésiterai tout de même pas à m'opposer à quelque chose de manifestement nuisible et qui se trouve sous mon nez)\n\n");
    }
  }
}
