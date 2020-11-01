using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
  // Keep in mind your module **must** be public and inherit ModuleBase.
  // If it isn't, it will not be discovered by AddModulesAsync!
  public class InfoModule : ModuleBase<SocketCommandContext>
  {
    // ~say hello world -> hello world
    [Command("say")]
    [Summary("Echoes a message.")]
    public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
      => ReplyAsync(echo);

    // ReplyAsync is a method on ModuleBase 

    [Command("whoishere")]
    [Summary("Affiche le nombre de joueurs connecté au module.")]
    public Task WhoIsHereAsync()
      => ReplyAsync($"Nous sommes actuellement {Utils.GetConnectedPlayers()} joueur(s) sur le module !");

    [Command("reboot")]
    [Summary("Reboot le module.")]
    public Task RebootAsync()
      => ReplyAsync(ModuleSystem.module.PreparingModuleForAsyncReboot());
  }
}
