using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetConnectedPlayersCommand(SocketCommandContext context)
    {
      await NwTask.SwitchToMainThread();

      if (DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id) == "admin")
      {
        string message = "";
        foreach (NwPlayer player in NwModule.Instance.Players)
          message += player.LoginCreature.Name + "\n";

        if (message.Length == 0)
          message = "Aucun joueur n'est actuellement connecté.";

        await context.Channel.SendMessageAsync(message);
      }
      else
        await context.Channel.SendMessageAsync($"Nous sommes actuellement {NwModule.Instance.Players.Count()} joueur(s) sur le module !");
    }
  }
}
