using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDisplayBacklogInfoCommand(SocketCommandContext context)
    {
      await context.Channel.SendMessageAsync("Backlog de projets en attente\n" +
        "    - Backlog\n" +
        "    - Craft menuiserie / ébeniste\n" +
        "    - Craft tannerie\n" +
        "    - Boutiques PJs\n" +
        "    - Malus de mort\n" +
        "    - Système de durabilité des objets\n" +
        "    - Système de lycanthropie(et autres malédictions)\n" +
        "    - Métier recherche d'enchantement\n" +
        "    - Systéme d'amélioration des items craftés (application des enchantements)\n" +
        "    - Système de remboursement de la dette en cas de non solvabilité\n" +
        "    - Système de repos\n" +
        "    - Système de transport de matières premières\n" +
        "    - Métier Craft parchemins\n" +
        "    - Métier Craft potions\n" +
        "    - Métier Craft baguettes\n" +
        "    - Métier Craft meubles persistants(via menuiserie / ébenisterie)\n" +
        "    - Système de messagerie + écriture de livre\n" +
        "    - Métier 'Professeur'\n" +
        "    - Système de déguisement\n" +
        "    - Renommer ses invocations\n" +
        "    - Faire parler ses invocations\n" +
        "    - Système de faim / soif(apporte bonus apprentissage / craft)\n" +
        "    - Système de repas d'auberge/taverne (différents coûts/qualité/bonus)\n" +
        "    - Métier cuisinier\n" +
        "    - Système d'amélioration des outils de craft\n" +
        "    - Système de couleurs de chat ?\n" +
        "    - Système de factions\n" +
        "    - Coloration spécifique des emotes(texte rp entre * *)\n" +
        "    - Permettre destruction de cadavres PJs\n" +
        "    - Canal groupe rp (autres pjs à portée d'oreille peuvent entendre)\n" +
        "    - Système de personnalisation d'apparence de vêtements (seul le crafteur initial pourra modifier)\n" +
        "    - Cooldowns sur dons activables(renversement, furtif, etc)\n" +
        "    - Amélioration minage(différentes qualités de filon identifiables par prospection)\n" +
        "    - Amélioration craft d'objets (ajouter talent qui donne % chance de réduire poids, augmenter durabilité et nb slots enchantements)\n" +
        "    - Talent épique permettant aux archers de ne pas subir d'AOO\n" +
        "    - Système 'savoir rp' (livres à apprendre)\n" +
        "    - Talent permettant d'ajouter un sort à une attaque\n" +
        "    - Système de persistance des sorts\n" +
        "    - Réfléchir à que faire de Utilisation des Objets Magiques\n" +
        "    - Réfléchir à que faire de Persuasion\n" +
        "    - Réfléchir à que faire d'Equitation (et des montures en général)\n"
        );

      await context.Channel.SendMessageAsync("\n    - Système anti-déconnexion sauvage\n\n" +
        "    - Système d'housing\n" +
        "    - Système climat & météo\n" +
        "    - Système de religion (piété et attribut tutélaire)\n" +
        "    - Système capture de monstre sans le tuer\n" +
        "    - Système capture de PJ sans le tuer\n" +
        "    - Amélioration de l'intelligence artificielle des monstres & PNJs\n" +
        "    - Système de justice (points civiques, https://gitlab.com/nwnee/attack-on-ancients/-/issues/68)\n" +
        "    - Achat / Vente via Discord\n" +
        "    - Suppression de personnage via Discord\n" +
        "    - Ajout de partitions permettant d'apprendre des chants du barde avec différents effets\n" +
        "    - Système de primes (bounty hunter)\n" +
        "    - Système de missions instanciées\n" +
        "    - Système arène automatique PvE\n" +
        "    - Système arène automatique PvP\n" +
        "    - Jeu de dé The Witcher\n" +
        "    - Casino (dés, blackjack, poker, etc)\n" +
        "    - Ajout nouveaux sorts\n" +
        "    - Ajout nouveaux dons/talents\n" +
        "    - Equilibrage des sorts\n" +
        "    - Equilibrage des dons/talents\n" +
        "    - Ajout système de corruption/purification des zones\n" +
        "    - Ajout système de pistage sur les maps\n" +
        "    - Ajout système d'assurance\n"
        );
    }
  }
}
