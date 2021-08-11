using System.Threading.Tasks;
using Discord.Commands;

namespace BotSystem
{
  // Keep in mind your module **must** be public and inherit ModuleBase.
  // If it isn't, it will not be discovered by AddModulesAsync!
  public class InfoModule : ModuleBase<SocketCommandContext>
  {
    // ~say hello world -> hello world
    /*[Command("say")]
    [Summary("Echoes a message.")]
    public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
      => ReplyAsync(echo);*/

    [Command("help")]
    [Summary("Affiche la liste des commandes disponibles.")]
    public async Task HelpAsync()
      => await BotCommand.ExecuteHelpCommandAsync(Context);

    [Command("register")]
    [Summary("Enregistre votre compte Discord sur le module en fonction de la clef cd fournie.")]
    public async Task RegisterDiscordId(string cdKey)
      => await BotCommand.ExecuteRegisterDiscordId(Context, cdKey);

    [Command("say")]
    [Summary("Permet de faire prononcer à votre personnage un message en jeu tout en profitant de l'édition de texte de Discord.")]
    public async Task SayAsync(string nom_du_personnage, string message_a_prononcer)
      => await BotCommand.ExecuteSayCommand(Context, nom_du_personnage, message_a_prononcer);

    // ReplyAsync is a method on ModuleBase 

    [Command("quiestla")]
    [Summary("Affiche le nombre de joueurs connectés au module.")]
    public async Task WhoIsHereAsync()
      => await BotCommand.ExecuteGetConnectedPlayersCommand(Context);

    [Command("demandestaff")]
    [Summary("Envoie votre demande directement sur le canal privé du staff.")]
    public async Task StaffRequestAsync(string texte_de_la_demande)
      => await BotCommand.ExecuteSendRequestToStaffCommand(Context, texte_de_la_demande);

    [Command("desc")]
    [Summary("Enregistre ou modifie une description pour un personnage donné.")]
    public async Task SaveDescriptionAsync(string nom_du_perso, string nom_description, string texte_description)
      => await BotCommand.ExecuteSaveDescriptionCommand(Context, nom_du_perso, nom_description, texte_description);

    [Command("desc")]
    [Summary("Affiche la liste des descriptions enregistrées pour un personnage donné.")]
    public async Task GetDescriptionListAsync(string nom_perso)
      => await BotCommand.ExecuteGetDescriptionListCommand(Context, nom_perso);

    [Command("desc")]
    [Summary("Affiche le contenu de la description demandée pour un personnage donné.")]
    public async Task GetDescriptionAsync(string nom_perso, string nom_description)
      => await BotCommand.ExecuteGetDescriptionCommand(Context, nom_perso, nom_description);

    [Command("supdesc")]
    [Summary("Supprime la description demandée pour un personnage donné.")]
    public async Task DeleteDescriptionAsync(string nom_perso, string nom_description)
      => await BotCommand.ExecuteDeleteDescriptionCommand(Context, nom_perso, nom_description);

    [Command("areadesc")]
    [Summary("Enregistre ou modifie la description pour une zone donnée à partir de son tag.")]
    public async Task SaveAreaDescriptionAsync(string tag_zone, string texte_description)
          => await BotCommand.ExecuteSaveAreaDescriptionCommand(Context, tag_zone, texte_description);

    [Command("rumeur")]
    [Summary("Enregistre ou modifie une rumeur.")]
    public async Task SaveRumorAsync(string titre_rumeur, string contenu_rumeur)
      => await BotCommand.ExecuteSaveRumorCommand(Context, titre_rumeur, contenu_rumeur);

    [Command("rumeur")]
    [Summary("Affiche la liste de vos rumeurs actives.")]
    public async Task GetMyRumorsListAsync()
      => await BotCommand.ExecuteGetMyRumorsListCommand(Context);

    [Command("rumeur")]
    [Summary("Affiche le contenu de la rumeur demandée.")]
    public async Task GetRumorAsync(int numero_rumeur)
      => await BotCommand.ExecuteGetRumorCommand(Context, numero_rumeur);

    [Command("suprumeur")]
    [Summary("Supprime la rumeur indiquée.")]
    public async Task DeleteRumorAsync(string numero_rumeur)
      => await BotCommand.ExecuteDeleteRumorCommand(Context, numero_rumeur);

    [Command("listepersos")]
    [Summary("Affiche la liste des personnages.")]
    public async Task GetCharacterListAsync()
      => await BotCommand.ExecuteGetCharacterListCommand(Context);

    [Command("envoi_courrier")]
    [Summary("Envoi un courrier au personnage indiqué.")]
    public async Task SendMailAsync(string nom_perso_emetteur, int ID_perso_destinataire, string titre_courrier, string contenu_courrier)
      => await BotCommand.ExecuteSendMailCommand(Context, nom_perso_emetteur, ID_perso_destinataire, titre_courrier, contenu_courrier);

    [Command("mycourrier")]
    [Summary("Affiche la liste des courriers destinés à l'un de vos personnages.")]
    public async Task GetMyMailsListAsync(string nom_perso)
      => await BotCommand.ExecuteGetMyMailsListCommand(Context, nom_perso);

    [Command("mycourrier")]
    [Summary("Affiche le contenu de la rumeur demandée.")]
    public async Task GetMailAsync(string numero_courrier, string nom_personnage)
      => await BotCommand.ExecuteGetMailCommand(Context, numero_courrier, nom_personnage);

    [Command("suprumeur")]
    [Summary("Supprime la rumeur indiquée.")]
    public async Task DeleteMailAsync(string numero_courrier, string nom_personnage)
      => await BotCommand.ExecuteDeleteMailCommand(Context, numero_courrier, nom_personnage);

    [Command("reboot")]
    [Summary("Reboot le module.")]
    public async Task RebootAsync()
          => await BotCommand.ExecuteRebootCommand(Context);

    [Command("refill")]
    [Summary("Lié à reboot.")]
    public async Task RefillAsync()
      => await BotCommand.ExecuteRefillCommand(Context);

    [Command("requete")]
    [Summary("Commande dm.")]
    public async Task HandleDBRequestAsync(string content)
      => await BotCommand.ExecuteDBRequestCommand(Context, content);

    [Command("univers")]
    [Summary("Affiche des informations au sujet de l'univers des Larmes des Erylies et de l'arrivée des joueurs dans l'archipel.")]
    public async Task DisplayUniverseInfo()
      => await BotCommand.ExecuteDisplayUniverseInfoCommand(Context);

    [Command("leitmotiv")]
    [Summary("Affiche des informations sur les principes fondateurs des Larmes des Erylies.")]
    public async Task DisplayLeitmotivInfo()
      => await BotCommand.ExecuteDisplayLeitmotivInfoCommand(Context);

    [Command("pvp")]
    [Summary("Affiche les principales règles liées au PvP sur les Larmes des Erylies.")]
    public async Task DisplayPVPInfo()
      => await BotCommand.ExecuteDisplayPVPInfoCommand(Context);

    [Command("mort")]
    [Summary("Affiche les principales règles liées aux différents types de morts sur les Larmes des Erylies.")]
    public async Task DisplayDeathInfo()
      => await BotCommand.ExecuteDisplayDeathInfoCommand(Context);

    [Command("soins")]
    [Summary("Affiche les principales informations sur la façon dont les soins sont pris en compte sur les Larmes des Erylies.")]
    public async Task DisplayHealInfo()
      => await BotCommand.ExecuteDisplayHealInfoCommand(Context);

    [Command("multicompte")]
    [Summary("Affiche les règles concernant le multi-compte sur les Larmes des Erylies.")]
    public async Task DisplayMultiAccountInfo()
      => await BotCommand.ExecuteDisplayMultiAccountInfoCommand(Context);

    [Command("savoir")]
    [Summary("Affiche les règles concernant le savoir sur les Larmes des Erylies.")]
    public async Task DisplayKnowledgeInfo()
      => await BotCommand.ExecuteDisplayKnowledgeInfoCommand(Context);

    [Command("dés")]
    [Summary("Affiche les règles concernant la prise en compte des jets de dés sur les Larmes des Erylies.")]
    public async Task DisplayDiceRollInfo()
      => await BotCommand.ExecuteDisplayDiceRollInfoCommand(Context);

    [Command("alignement")]
    [Summary("Affiche des informations concernant la façon dont l'alignement est utilisé sur les Larmes des Erylies.")]
    public async Task DisplayAlignementInfo()
      => await BotCommand.ExecuteDisplayAlignementInfoCommand(Context);

    [Command("evil")]
    [Summary("Affiche des recommandations sur la façon de jouer evil sur les Larmes des Erylies sans risquer trop de frustration.")]
    public async Task DisplayEvilInfo()
      => await BotCommand.ExecuteDisplayEvilInfoCommand(Context);

    [Command("bonusroleplay")]
    [Summary("Affiche le principe et fonctionnement du bonus de roleplay.")]
    public async Task DisplayBonusRoleplayInfo()
      => await BotCommand.ExecuteDisplayBonusRoleplayInfoCommand(Context);

    [Command("sortsrp")]
    [Summary("Affiche les règles concernant la gestion des sorts rp sur les Larmes des Erylies.")]
    public async Task DisplayRoleplaySpellsInfo()
      => await BotCommand.ExecuteDisplayRoleplaySpellsInfoCommand(Context);

    [Command("magie")]
    [Summary("Affiche des informations concernant la façon dont la magie fonctionne sur les Larmes des Erylies.")]
    public async Task DisplayMagicInfo()
      => await BotCommand.ExecuteDisplayMagicInfoCommand(Context);

    [Command("animations")]
    [Summary("Affiche des informations concernant le fonctionnement général des animations sur les Larmes des Erylies.")]
    public async Task DisplayAnimationInfo()
      => await BotCommand.ExecuteDisplayAnimationInfoCommand(Context);

    [Command("developpement")]
    [Summary("Affiche la liste des développements et tâches en cours.")]
    public async Task DisplayDevInfo()
      => await BotCommand.ExecuteDisplayDevInfoCommand(Context);

    [Command("backlog")]
    [Summary("Affiche la liste des projets et tâches en attente.")]
    public async Task DisplayBacklogInfo()
      => await BotCommand.ExecuteDisplayBacklogInfoCommand(Context);
  }
}
