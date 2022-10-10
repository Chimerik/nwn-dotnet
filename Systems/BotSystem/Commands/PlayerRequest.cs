using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Discord.WebSocket;
using System.Linq;
using Discord;
using Discord.Rest;
using System.ComponentModel.DataAnnotations;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecutePlayerRequestCommand(SocketSlashCommand command)
    {
      string title = command.Data.Options.First().Value.ToString();

      SocketCategoryChannel category = Bot.discordServer.GetCategoryChannel(1026486893136855124);

      if(!Bot.discordServer.Roles.Any(r => r.Name == command.User.Username))
      {
        

        if (!category.Channels.Any(c => c.Name == command.User.Username))
        {
          RestTextChannel chan = await Bot.discordServer.CreateTextChannelAsync(command.User.Username);
          var customPermissions = new GuildPermission();
          RestRole userRole = await Bot.discordServer.CreateRoleAsync(command.User.Username, GuildPermission.ViewChannel);
          await (command.User as IGuildUser).AddRoleAsync(userRole);
        }
      }



      if (!category.Channels.Any(c => c.Name == command.User.Username))
      {
        RestTextChannel chan = await Bot.discordServer.CreateTextChannelAsync(command.User.Username);

      }
      else
      {
        SocketGuildChannel channel = category.Channels.First(c => c.Name == command.User.Username);
      }

      //await chan.CreateThreadAsync(title);

      //Bot._client.GetChannel(703964971549196339).

      // RestThreadChannel post = await Bot.logChannel.forum.CreatePostAsync($"{command.User.Username} : {command.Data.Options.First().Value.ToString()}", ThreadArchiveDuration.OneWeek, null, command.Data.Options.ElementAt(1).Value.ToString());
      //await post.AddUserAsync((IGuildUser)command.User);

      await command.RespondAsync("Demande staff crée !", ephemeral: true);
    }
  }
}
