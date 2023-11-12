using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Google.Apis.Drive.v3.Data;

namespace NWN.Systems
{
  public static partial class Bot
  {
    private static async Task OnUserJoinUpdateUserList(SocketGuildUser data)
    {
      await _client.DownloadUsersAsync(new List<IGuild> { { discordServer } });

      if (data.IsBot)
        return;

      try
      {
        await data.AddRoleAsync(DiscordConfig.RoleNaufrage);
        await playerGeneralChannel.SendMessageAsync($"Bonjour {data.Mention} et bienvenue sur le Discord des Larmes des Erylies.\n\n Pour commencer, n'hésitez pas à consulter notre livre du joueur : \n https://docs.google.com/document/d/1ammPGnH-sVjNHnJHCMAm_khbe8mBqTPFeCfqCvJt7ig/edit?usp=sharing");

        if (!discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory1).Channels.Any(c => c.Name == data.Username.ToLower().Replace(" ", "-"))
           && !discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory2).Channels.Any(c => c.Name == data.Username.ToLower().Replace(" ", "-"))
           && !discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory3).Channels.Any(c => c.Name == data.Username.ToLower().Replace(" ", "-")))
        {
          forumCategory = discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory1);

          if (forumCategory.Channels.Count > 49)
            forumCategory = discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory2);

          if (forumCategory.Channels.Count > 49)
            forumCategory = discordServer.GetCategoryChannel(DiscordConfig.PlayerForumCategory3);

          var chan = await discordServer.CreateForumChannelAsync(data.Username, f => { f.CategoryId = forumCategory.Id; });
          await chan.AddPermissionOverwriteAsync(data, requestForumPermissions);
          await chan.CreatePostAsync("Suivi personnalisé", ThreadArchiveDuration.OneWeek, null, $"Bonjour {data.Mention} !\n\n" +
            $"Bienvenue sur votre forum personnalisé de suivi de joueur. Ce forum et les sujets qu'il contient ne sont visibles que pour vous et le staff.\n\n" +
            $"Cela permettra donc de nous faire parvenir vos demandes RP et d'en suivre l'avancement, ainsi que de tenir au courant le staff des derniers développements de votre personnage !");
        }

        if (forumCategory.Channels.Count > 40)
          await chimDiscordUser.SendMessageAsync($"Attention : catégorie suivi de joueurs presque pleine. Pensez à en créer une nouvelle !");
      }
      catch (Exception e)
      {
        LogUtils.LogMessage($"{e}\n{e.Message}\n{e.StackTrace}", LogUtils.LogType.ModuleAdministration);
      }
    }
  }
}
