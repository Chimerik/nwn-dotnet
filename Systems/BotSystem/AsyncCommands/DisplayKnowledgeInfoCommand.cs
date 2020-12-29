using Discord.Commands;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static string ExecuteDisplayKnowledgeInfoCommand(SocketCommandContext context)
    {
      context.Channel.SendMessageAsync("**Savoir et connaissances rp :**\n\n" +
        "Afin de partir sur de bonnes bases communes, il est important de garder à l'esprit qu'à son arrivée, votre personnage est niveau 1 : un tel personnages ne sait **rien** à partir du moment où le sujet ne fait pas partie de son domaine d'expertise et qu'il n'a jamais été confronté en rp au sujet.\n\n" +
        "**Exemple :**\n\n" +
        "*Un prêtre de niveau 1 ne sait pas qu'un vampire vaincu retourne à son sarcophage pour se régénérer. Pour l'apprendre, il lui faudra en faire l'expérience et trouver le sarcophage d'un vampire.\n" +
        "* Un prêtre de Lathandre de niveau 1 confronté à un vampire vaincu aura la possibilité de faire un jet de savoir afin de déterminer s'il a accès à un peu plus d'informations quant à la nature de cette étrange brume qui s'enfuit au loin une fois le vampire terrassé.\n\n" +
        "On dit ici 'niveau 1', mais sur les Larmes, les niveaux n'existent pas réellement. Quoiqu'il en soit votre personnage ne sait rien tant qu'il n'a pas été confronté en rp au sujet. Dans le cas du prêtre non spécialisé dans la non - vie, il sera nécessaire de trouver le cercueil et de constater que la brume y a trouvé refuge afin de comprendre ce qu'il se passe et d'acquérir un nouveau savoir.\n\n" +
        "Idem pour les arcanistes : la connaissance est rare et coûte horriblement cher.\n\n" +
        "Vos études vous permettront d'en apprendre davantage, mais étant niveau 1 lorsque vous commencerez, vous n'aurez pas encore eu le temps d'accomplir de grandes prouesses, ni l'opportunité d'avoir accès à de précieux grimoires. Le savoir s'accumulera donc en jeu et en rp, en fonction des expériences et études menées par votre personnage ainsi que des éventuels ouvrages qu'il aura pu trouver.\n\n" +
        "**Exemple :**\n\n" +
        "*Un arcaniste pourra apprendre que les trolls régénèrent toutes les blessures dont la source n'est pas l'acide ou le feu s'il trouve en jeu un grimoire détaillant l'écologie des trolls.\n\n"
        );

      return "\n\n**Un stratège pourra obtenir cette même connaissance après avoir constaté en jeu qu'un troll se rétablissait constamment après un combat et après l'avoir capturé, puis soumis à tout un tas d'expérience afin de déterminer ses faiblesses.\n\n" +
        "Pour résumer : le savoir de votre personnage s'acquiert uniquement en jeu. La spécialité de votre personnage lui permet d'avoir certaines facilités pour l'acquisition de ce savoir. Le savoir ne s'acquiert jamais à partir de vos propres lectures et connaissances hrp.\n\n" +
        "A noter que les ouvrages de savoir n'existent pas pour le moment en jeu. Ceux-ci seront ajoutés plus tard et seront pris en compte dans le système de leveling.\n\n";
    }
  }
}
