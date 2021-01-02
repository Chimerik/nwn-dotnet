using System;
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
      => BotSystem.ExecuteHelpCommandAsync(Context);

    [Command("register")]
    [Summary("Enregistre votre compte Discord sur le module en fonction de la clef cd fournie.")]
    public Task RegisterDiscordId(string cdKey)
      => BotSystem.ExecuteRegisterDiscordId(Context, cdKey);

    [Command("say")]
    [Summary("Permet de faire prononcer à votre personnage un message en jeu tout en profitant de l'édition de texte de Discord.")]
    public Task SayAsync(string nom_du_personnage, string message_a_prononcer)
      => ModuleSystem.module.PreparingModuleForAsyncSay(Context, nom_du_personnage, message_a_prononcer);

    // ReplyAsync is a method on ModuleBase 

    [Command("quiestla")]
    [Summary("Affiche le nombre de joueurs connectés au module.")]
    public Task WhoIsHereAsync()
      => BotSystem.ExecuteGetConnectedPlayersCommand(Context);

    [Command("desc")]
    [Summary("Enregistre ou modifie une description pour un personnage donné.")]
    public Task SaveDescriptionAsync(string nom_du_perso, string nom_description, string texte_description)
      => BotSystem.ExecuteSaveDescriptionCommand(Context, nom_du_perso, nom_description, texte_description);

    [Command("desc")]
    [Summary("Affiche la liste des descriptions enregistrées pour un personnage donné.")]
    public Task GetDescriptionListAsync(string nom_perso)
      => BotSystem.ExecuteGetDescriptionListCommand(Context, nom_perso);

    [Command("desc")]
    [Summary("Affiche le contenu de la description demandée pour un personnage donné.")]
    public Task GetDescriptionAsync(string nom_perso, string nom_description)
      => BotSystem.ExecuteGetDescriptionCommand(Context, nom_perso, nom_description);

    [Command("remdesc")]
    [Summary("Supprime la description demandée pour un personnage donné.")]
    public Task DeleteDescriptionAsync(string nom_perso, string nom_description)
      => BotSystem.ExecuteDeleteDescriptionCommand(Context, nom_perso, nom_description);

    [Command("reboot")]
    [Summary("Reboot le module.")]
    public Task RebootAsync()
      => ModuleSystem.module.PreparingModuleForAsyncReboot(Context);

    [Command("univers")]
    [Summary("Affiche des informations au sujet de l'univers des Larmes des Erylies et de l'arrivée des joueurs dans l'archipel.")]
    public Task DisplayUniverseInfo()
      => BotSystem.ExecuteDisplayUniverseInfoCommand(Context);

    [Command("leitmotiv")]
    [Summary("Affiche des informations sur les principes fondateurs des Larmes des Erylies.")]
    public Task DisplayLeitmotivInfo()
      => BotSystem.ExecuteDisplayLeitmotivInfoCommand(Context);

    [Command("pvp")]
    [Summary("Affiche les principales règles liées au PvP sur les Larmes des Erylies.")]
    public Task DisplayPVPInfo()
      => BotSystem.ExecuteDisplayPVPInfoCommand(Context);

    [Command("mort")]
    [Summary("Affiche les principales règles liées aux différents types de morts sur les Larmes des Erylies.")]
    public Task DisplayDeathInfo()
      => BotSystem.ExecuteDisplayDeathInfoCommand(Context);

    [Command("soins")]
    [Summary("Affiche les principales informations sur la façon dont les soins sont pris en compte sur les Larmes des Erylies.")]
    public Task DisplayHealInfo()
      => BotSystem.ExecuteDisplayHealInfoCommand(Context);

    [Command("multicompte")]
    [Summary("Affiche les règles concernant le multi-compte sur les Larmes des Erylies.")]
    public Task DisplayMultiAccountInfo()
      => BotSystem.ExecuteDisplayMultiAccountInfoCommand(Context);

    [Command("savoir")]
    [Summary("Affiche les règles concernant le savoir sur les Larmes des Erylies.")]
    public Task DisplayKnowledgeInfo()
      => BotSystem.ExecuteDisplayKnowledgeInfoCommand(Context);

    [Command("dés")]
    [Summary("Affiche les règles concernant la prise en compte des jets de dés sur les Larmes des Erylies.")]
    public Task DisplayDiceRollInfo()
      => BotSystem.ExecuteDisplayDiceRollInfoCommand(Context);

    [Command("alignement")]
    [Summary("Affiche des informations concernant la façon dont l'alignement est utilisé sur les Larmes des Erylies.")]
    public Task DisplayAlignementInfo()
      => BotSystem.ExecuteDisplayAlignementInfoCommand(Context);

    [Command("evil")]
    [Summary("Affiche des recommandations sur la façon de jouer evil sur les Larmes des Erylies sans risquer trop de frustration.")]
    public Task DisplayEvilInfo()
      => BotSystem.ExecuteDisplayEvilInfoCommand(Context);

    [Command("bonusroleplay")]
    [Summary("Affiche le principe et fonctionnement du bonus de roleplay.")]
    public Task DisplayBonusRoleplayInfo()
      => BotSystem.ExecuteDisplayBonusRoleplayInfoCommand(Context);

    [Command("sortsrp")]
    [Summary("Affiche les règles concernant la gestion des sorts rp sur les Larmes des Erylies.")]
    public Task DisplayRoleplaySpellsInfo()
      => BotSystem.ExecuteDisplayRoleplaySpellsInfoCommand(Context);

    [Command("magie")]
    [Summary("Affiche des informations concernant la façon dont la magie fonctionne sur les Larmes des Erylies.")]
    public Task DisplayMagicInfo()
      => BotSystem.ExecuteDisplayMagicInfoCommand(Context);

    [Command("animations")]
    [Summary("Affiche des informations concernant le fonctionnement général des animations sur les Larmes des Erylies.")]
    public Task DisplayAnimationInfo()
      => BotSystem.ExecuteDisplayAnimationInfoCommand(Context);

    [Command("developpement")]
    [Summary("Affiche la liste des développements et tâches en cours.")]
    public Task DisplayDevInfo()
      => BotSystem.ExecuteDisplayDevInfoCommand(Context);

    [Command("backlog")]
    [Summary("Affiche la liste des projets et tâches en attente.")]
    public Task DisplayBacklogInfo()
      => BotSystem.ExecuteDisplayBacklogInfoCommand(Context);
  }
}
