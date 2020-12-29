using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static string ExecuteDisplayMultiAccountInfoCommand()
    {
      return "Le multi-compte n'est pas autorisé : 1 un joueur = 1 personnage afin de permettre de se concentrer  et de s'investir principalement sur le rp de ce personnage.\n\n" +
        "Pour commencer un nouveau personnage, il faut arrêter le précédent. Il est alors possible de bénéficier des règles d'obtention de points pour débloquer des races non disponibles de base à la création de personnage. Le système n'est pas encore en place et sera à définir.Il se fondera sur la quantité de points acquis par le personnage via le système de leveling.\n\n" +
        "Il est également possible, avec accord du staff, de mettre en pause un personnage afin d'en jouer un autre. La durée minimum de la pause est d'un mois. Pendant la pause, il n'est plus possible de jouer le précédent personnage. Après la pause, il n'est plus possible de jouer le nouveau personnage.\n\n" +
        "Afin de permettre aux joueurs ayant acheté plusieurs CD - KEY d'en bénéficier, il sera donné la possibilité d'incarner des personnages secondaires mineurs de type:\n" +
        "    - Animal de compagnie(chien, chat, etc)\n" +
        "    - Famille(père, mère, enfant, etc)\n" +
        "    - Familier / Compagnon animal\n" +
        "    - Récompense rp type garde du corps ou apprentis\n\n" +
      "Ce type de personnage ne peut s'acquérir qu'avec accord du staff et doit obligatoirement avoir un rôle d'enrichissement du rp du personnage principal. Leurs stats techniques seront très inférieures à celles d'un pj normal et ils ne pourront utiliser ni le système de leveling ni celui de craft.\n\n";
    }
  }
}
