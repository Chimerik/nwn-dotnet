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
    {
      var msgs = BotSystem.ExecuteHelpCommand();

      foreach (var msg in msgs)
      {
        await ReplyAsync(msg);
      }
    }

    [Command("say")]
    [Summary("Echoes a message.")]
    public Task SayAsync(string spcName, string text)
      => ReplyAsync(ModuleSystem.module.PreparingModuleForAsyncSay(Context, spcName, text));

    // ReplyAsync is a method on ModuleBase 

    [Command("whoishere")]
    [Summary("Affiche le nombre de joueurs connectés au module.")]
    public Task WhoIsHereAsync()
      => ReplyAsync($"Nous sommes actuellement {BotSystem.ExecuteGetConnectedPlayersCommand()} joueur(s) sur le module !");

    [Command("desc")]
    [Summary("Enregistre ou modifie une description pour un personnage donné.")]
    public Task SaveDescriptionAsync(string spcName, string name, string text)
      => ReplyAsync(BotSystem.ExecuteSaveDescriptionCommand(Context, spcName, name, text));

    [Command("desc")]
    [Summary("Affiche la liste des descriptions enregistrées pour un personnage donné.")]
    public Task GetDescriptionListAsync(string spcName)
      => ReplyAsync(BotSystem.ExecuteGetDescriptionListCommand(Context, spcName));

    [Command("desc")]
    [Summary("Affiche le contenu de la description demandée pour un personnage donné.")]
    public Task GetDescriptionAsync(string spcName, string name)
      => ReplyAsync(BotSystem.ExecuteGetDescriptionCommand(Context, spcName, name));

    [Command("remdesc")]
    [Summary("Supprime la description demandée pour un personnage donné.")]
    public Task DeleteDescriptionAsync(string spcName, string name)
      => ReplyAsync(BotSystem.ExecuteDeleteDescriptionCommand(Context, spcName, name));

    [Command("reboot")]
    [Summary("Reboot le module.")]
    public Task RebootAsync()
      => ReplyAsync(ModuleSystem.module.PreparingModuleForAsyncReboot(Context));

    [Command("register")]
    [Summary("Enregistre votre compte Discord sur le module en fonction de la clef cd fournie.")]
    public Task RegisterDiscordId(string cdKey)
      => ReplyAsync($"{BotSystem.ExecuteRegisterDiscordId(Context, cdKey)}");

    [Command("univers")]
    [Summary("Affiche des informations au sujet de l'univers des Larmes des Erylies et de l'arrivée des joueurs dans l'archipel.")]
    public Task DisplayUniverseInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayUniverseInfoCommand(Context)}");

    [Command("leitmotiv")]
    [Summary("Affiche des informations sur les principes fondateurs des Larmes des Erylies.")]
    public Task DisplayLeitmotivInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayLeitmotivInfoCommand(Context)}");

    [Command("pvp")]
    [Summary("Affiche les principales règles liées au PvP sur les Larmes des Erylies.")]
    public Task DisplayPVPInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayPVPInfoCommand(Context)}");

    [Command("mort")]
    [Summary("Affiche les principales règles liées aux différents types de morts sur les Larmes des Erylies.")]
    public Task DisplayDeathInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayDeathInfoCommand(Context)}");

    [Command("soins")]
    [Summary("Affiche les principales informations sur la façon dont les soins sont pris en compte sur les Larmes des Erylies.")]
    public Task DisplayHealInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayHealInfoCommand(Context)}");

    [Command("multicompte")]
    [Summary("Affiche les règles concernant le multi-compte sur les Larmes des Erylies.")]
    public Task DisplayMultiAccountInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayMultiAccountInfoCommand()}");

    [Command("savoir")]
    [Summary("Affiche les règles concernant le savoir sur les Larmes des Erylies.")]
    public Task DisplayKnowledgeInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayKnowledgeInfoCommand(Context)}");

    [Command("dés")]
    [Summary("Affiche les règles concernant le savoir sur les Larmes des Erylies.")]
    public Task DisplayDiceRollInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayDiceRollInfoCommand(Context)}");

    [Command("alignement")]
    [Summary("Affiche des informations concernant la façon dont l'alignement est utilisé sur les Larmes des Erylies.")]
    public Task DisplayAlignementInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayAlignementInfoCommand(Context)}");

    [Command("evil")]
    [Summary("Affiche des recommandations sur la façon de jouer evil sur les Larmes des Erylies sans risquer trop de frustration.")]
    public Task DisplayEvilInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayEvilInfoCommand(Context)}");

    [Command("bonusroleplay")]
    [Summary("Affiche le principe et fonctionnement du bonus de roleplay.")]
    public Task DisplayBonusRoleplayInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayBonusRoleplayInfoCommand(Context)}");

    [Command("sortsrp")]
    [Summary("Affiche les règles concernant la gestion des sorts rp sur les Larmes des Erylies.")]
    public Task DisplayRoleplaySpellsInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayRoleplaySpellsInfoCommand()}");

    [Command("magie")]
    [Summary("Affiche des informations concernant la façon dont la magie fonctionne sur les Larmes des Erylies.")]
    public Task DisplayMagicInfo()
      => ReplyAsync($"{BotSystem.ExecuteDisplayMagicInfoCommand(Context)}");
  }
}
