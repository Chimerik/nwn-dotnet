using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteRegisterDiscordId(SocketCommandContext context, string cdKey)
    {
      await NwTask.SwitchToMainThread();

      SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "discordId", context.User.Id.ToString() } },
          new List<string[]>() { new string[] { "cdKey", cdKey } });

      await context.Channel.SendMessageAsync("Voilà qui est fait. Enfin, pour tant soit peu que la clef fournie fusse valide !");
    }
  }
}
