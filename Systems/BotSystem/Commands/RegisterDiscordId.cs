using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteRegisterDiscordId(SocketSlashCommand command)
    {
      SqLiteUtils.UpdateQuery("PlayerAccounts",
          new List<string[]>() { new string[] { "discordId", command.User.Id.ToString() } },
          new List<string[]>() { new string[] { "cdKey", command.Data.Options.First().Value.ToString() } });

      await command.RespondAsync("Discord a bien été lié à la clef fournie. Enfin, pour tant soit peu que la clef fournie fusse valide !", ephemeral: true);
    }
  }
}
