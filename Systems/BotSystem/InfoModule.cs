﻿using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
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
      => await BotSystem.ExecuteHelpCommandAsync(Context);

    [Command("register")]
    [Summary("Enregistre votre compte Discord sur le module en fonction de la clef cd fournie.")]
    public async Task RegisterDiscordId(string cdKey)
      => await BotSystem.ExecuteRegisterDiscordId(Context, cdKey);

    /*[Command("say")]
    [Summary("Permet de faire prononcer à votre personnage un message en jeu tout en profitant de l'édition de texte de Discord.")]
    public async Task SayAsync(string nom_du_personnage, string message_a_prononcer)
      => await BotSystem.ExecuteSayCommand(Context, nom_du_personnage, message_a_prononcer);*/

    // ReplyAsync is a method on ModuleBase 

    [Command("quiestla")]
    [Summary("Affiche le nombre de joueurs connectés au module.")]
    public async Task WhoIsHereAsync()
      => await BotSystem.ExecuteGetConnectedPlayersCommand(Context);

    [Command("demandestaff")]
    [Summary("Envoie votre demande directement sur le canal privé du staff.")]
    public async Task StaffRequestAsync(string texte_de_la_demande)
      => await BotSystem.ExecuteSendRequestToStaffCommand(Context, texte_de_la_demande);

    [Command("rumeur")]
    [Summary("Enregistre ou modifie une rumeur.")]
    public async Task SaveRumorAsync(string titre_rumeur, string contenu_rumeur)
      => await BotSystem.ExecuteSaveRumorCommand(Context, titre_rumeur, contenu_rumeur);

    [Command("rumeur")]
    [Summary("Affiche la liste de vos rumeurs actives.")]
    public async Task GetMyRumorsListAsync()
      => await BotSystem.ExecuteGetMyRumorsListCommand(Context);

    [Command("rumeur")]
    [Summary("Affiche le contenu de la rumeur demandée.")]
    public async Task GetRumorAsync(int numero_rumeur)
      => await BotSystem.ExecuteGetRumorCommand(Context, numero_rumeur);

    /*[Command("listepersos")]
    [Summary("Affiche la liste des personnages.")]
    public async Task GetCharacterListAsync()
      => await BotSystem.ExecuteGetCharacterListCommand(Context);*/

    /*[Command("envoi_courrier")]
    [Summary("Envoi un courrier au personnage indiqué.")]
    public async Task SendMailAsync(string nom_perso_emetteur, int ID_perso_destinataire, string titre_courrier, string contenu_courrier)
      => await BotSystem.ExecuteSendMailCommand(Context, nom_perso_emetteur, ID_perso_destinataire, titre_courrier, contenu_courrier);*/

    /*[Command("mycourrier")]
    [Summary("Affiche la liste des courriers destinés à l'un de vos personnages.")]
    public async Task GetMyMailsListAsync(string nom_perso)
      => await BotSystem.ExecuteGetMyMailsListCommand(Context, nom_perso);

    [Command("mycourrier")]
    [Summary("Affiche le contenu du courrier spécifié.")]
    public async Task GetMailAsync(string numero_courrier, string nom_personnage)
      => await BotSystem.ExecuteGetMailCommand(Context, numero_courrier, nom_personnage);*/

    [Command("reboot")]
    [Summary("Reboot le module.")]
    public async Task RebootAsync()
          => await BotSystem.ExecuteRebootCommand(Context);

    [Command("refill")]
    [Summary("Lié à reboot.")]
    public async Task RefillAsync()
      => await BotSystem.ExecuteRefillCommand(Context);

    [Command("requete")]
    [Summary("Commande dm.")]
    public async Task HandleDBRequestAsync(string content)
      => await BotSystem.ExecuteDBRequestCommand(Context, content);

    [Command("univers")]
    [Summary("Affiche des informations au sujet de l'univers des Larmes des Erylies et de l'arrivée des joueurs dans l'archipel.")]
    public async Task DisplayUniverseInfo()
      => await BotSystem.ExecuteDisplayUniverseInfoCommand(Context);

    [Command("leitmotiv")]
    [Summary("Affiche des informations sur les principes fondateurs des Larmes des Erylies.")]
    public async Task DisplayLeitmotivInfo()
      => await BotSystem.ExecuteDisplayLeitmotivInfoCommand(Context);

    [Command("pvp")]
    [Summary("Affiche les principales règles liées au PvP sur les Larmes des Erylies.")]
    public async Task DisplayPVPInfo()
      => await BotSystem.ExecuteDisplayPVPInfoCommand(Context);

    [Command("multicompte")]
    [Summary("Affiche les règles concernant le multi-compte sur les Larmes des Erylies.")]
    public async Task DisplayMultiAccountInfo()
      => await BotSystem.ExecuteDisplayMultiAccountInfoCommand(Context);

    [Command("savoir")]
    [Summary("Affiche les règles concernant le savoir sur les Larmes des Erylies.")]
    public async Task DisplayKnowledgeInfo()
      => await BotSystem.ExecuteDisplayKnowledgeInfoCommand(Context);

    [Command("sortsrp")]
    [Summary("Affiche les règles concernant la gestion des sorts rp sur les Larmes des Erylies.")]
    public async Task DisplayRoleplaySpellsInfo()
      => await BotSystem.ExecuteDisplayRoleplaySpellsInfoCommand(Context);

    [Command("magie")]
    [Summary("Affiche des informations concernant la façon dont la magie fonctionne sur les Larmes des Erylies.")]
    public async Task DisplayMagicInfo()
      => await BotSystem.ExecuteDisplayMagicInfoCommand(Context);
  }
}
