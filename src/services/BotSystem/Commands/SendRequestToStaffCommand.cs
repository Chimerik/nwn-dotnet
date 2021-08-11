using System.Threading.Tasks;
using Discord.Commands;

using Utils;

namespace BotSystem
{
  public static partial class BotCommand
  {
    public static async Task ExecuteSendRequestToStaffCommand(SocketCommandContext context, string demande)
    {
      await (DiscordUtils._client.GetChannel(796027712510492674) as Discord.IMessageChannel).SendMessageAsync($"{context.User.Username} : {demande}");
    }
  }
}
