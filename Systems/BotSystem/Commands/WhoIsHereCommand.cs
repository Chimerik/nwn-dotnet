using System.Linq;
using System.Threading.Tasks;
using Anvil.API;
using Discord.WebSocket;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetConnectedPlayersCommand(SocketSlashCommand command, bool staff = false)
    {
      await NwTask.SwitchToMainThread();

      if (staff)
      {
        string message = "";
        foreach (NwPlayer player in NwModule.Instance.Players)
          message += player.LoginCreature.Name + "\n";

        if (message.Length == 0)
          message = "Aucun joueur n'est actuellement connecté.";

        await command.RespondAsync(message, ephemeral:true);
      }
      else
        await command.RespondAsync($"Nous sommes actuellement {NwModule.Instance.Players.Count()} joueur(s) sur le module !", ephemeral: true);
    }
  }
}
