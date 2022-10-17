using System.Threading.Tasks;
using Discord.WebSocket;
using System.Linq;
using Discord.Rest;
using System;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecutePlayerRequestCommand(SocketSlashCommand command)
    {
      string title = command.Data.Options.First().Value.ToString();

      try
      {
        if (!Bot.forumCategory.Channels.Any(c => c.Name == command.User.Username))
        {
          RestTextChannel chan = await Bot.discordServer.CreateTextChannelAsync(command.User.Username, f => { f.CategoryId = Bot.forumCategory.Id; });
          await chan.AddPermissionOverwriteAsync(command.User, Bot.requestForumPermissions);
          await command.RespondAsync("Votre salon privé de demande au staff a été créé !", ephemeral: true);
        }
        else
          await command.RespondAsync("Votre salon privé de demande existe déjà !", ephemeral: true);
      }
      catch(Exception e)
      {
        Utils.LogMessageToDMs($"{e.Message}\n{e.StackTrace}");
      }

      // RestThreadChannel post = await Bot.logChannel.forum.CreatePostAsync($"{command.User.Username} : {command.Data.Options.First().Value.ToString()}", ThreadArchiveDuration.OneWeek, null, command.Data.Options.ElementAt(1).Value.ToString());
      //await post.AddUserAsync((IGuildUser)command.User);
    }
  }
}
