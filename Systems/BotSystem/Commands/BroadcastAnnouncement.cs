using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Discord.WebSocket;
using System.Linq;
using Discord;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteBroadcastAnnouncementCommand(SocketSlashCommand command)
    {
      string annonce = command.Data.Options.First().Value.ToString();
      IMessageChannel channel = Bot._client.GetChannel(1026481714895278161) as IMessageChannel; 

      if (command.Data.Options.Count > 1)
        channel = (IMessageChannel)command.Data.Options.ElementAt(1).Value;

      await channel.SendMessageAsync(annonce);
      await command.RespondAsync("Annonce publiée", ephemeral: true);
    }
  }
}
