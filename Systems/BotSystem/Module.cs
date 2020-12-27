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

    [Command("say")]
    [Summary("Echoes a message.")]
    public Task SayAsync(string spcName, string text)
      => ReplyAsync(ModuleSystem.module.PreparingModuleForAsyncSay(Context, spcName, text));

    // ReplyAsync is a method on ModuleBase 

    [Command("whoishere")]
    [Summary("Affiche le nombre de joueurs connectés au module.")]
    public Task WhoIsHereAsync()
      => ReplyAsync($"Nous sommes actuellement {Utils.GetConnectedPlayers()} joueur(s) sur le module !");

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
      => ReplyAsync($"{Utils.RegisterDiscordId(Context, cdKey)}");
  }
}
