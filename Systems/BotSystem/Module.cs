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
    public Task SayAsync()
      => ReplyAsync(ModuleSystem.module.PreparingModuleForAsyncSay(Context));

    // ReplyAsync is a method on ModuleBase 

    [Command("whoishere")]
    [Summary("Affiche le nombre de joueurs connecté au module.")]
    public Task WhoIsHereAsync()
      => ReplyAsync($"Nous sommes actuellement {Utils.GetConnectedPlayers()} joueur(s) sur le module !");

    [Command("reboot")]
    [Summary("Reboot le module.")]
    public Task RebootAsync()
      => ReplyAsync(ModuleSystem.module.PreparingModuleForAsyncReboot());

    [Command("register")]
    [Summary("Enregistre votre compte Discord sur le module en fonction de la clef cd fournie.")]
    public Task RegisterDiscordId(string cdKey)
      => ReplyAsync($"Vous êtes {Utils.RegisterDiscordId(Context, cdKey)} !");
  }
}
