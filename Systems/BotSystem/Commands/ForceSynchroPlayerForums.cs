using Anvil.API;
using System.Threading.Tasks;
using Discord.WebSocket;
using System;
using System.Linq;
using Discord;

namespace NWN.Systems
{
  public static partial class Bot
  {
    public static async Task ExecuteForceForumsCommand(SocketSlashCommand command)
    {
      foreach(var user in discordServer.Users)
      {
        if (user.IsBot)
          continue;

        try
        {
          if (!discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory1).Channels.Any(c => c.Name == user.Username.ToLower().Replace(" ", "-"))
           && !discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory2).Channels.Any(c => c.Name == user.Username.ToLower().Replace(" ", "-"))
           && !discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory3).Channels.Any(c => c.Name == user.Username.ToLower().Replace(" ", "-")))
          {
            forumCategory = discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory1);

            if (forumCategory.Channels.Count > 49)
              forumCategory = discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory2);

            if (forumCategory.Channels.Count > 49)
              forumCategory = discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory3);

            var chan = await discordServer.CreateForumChannelAsync(user.Username, f => { f.CategoryId = forumCategory.Id; });
            await chan.AddPermissionOverwriteAsync(user, requestForumPermissions);
            await chan.CreatePostAsync("Suivi personnalisé", ThreadArchiveDuration.OneWeek, null, $"Bonjour {user.Mention} !\n\n" +
              $"Bienvenue sur votre forum personnalisé de suivi de joueur. Ce forum et les sujets qu'il contient ne sont visibles que pour vous et le staff.\n\n" +
              $"Cela permettra donc de nous faire parvenir vos demandes RP et d'en suivre l'avancement, ainsi que de tenir au courant le staff des derniers développements de votre personnage !");

            LogUtils.LogMessage($"Création du forum personnel de {user.DisplayName}", LogUtils.LogType.ModuleAdministration);
          }
          else
            LogUtils.LogMessage($"Forum personnel de {user.DisplayName} => Déjà existant", LogUtils.LogType.ModuleAdministration);
        }
        catch (Exception e)
        {
          LogUtils.LogMessage($"Echec de la création du forum personnel de {user.DisplayName}" +
            $"{e}\n{e.Message}\n{e.StackTrace}", LogUtils.LogType.ModuleAdministration);
        }
      }
    }
  }
}
