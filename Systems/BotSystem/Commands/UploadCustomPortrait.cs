using System.Threading.Tasks;
using Discord.WebSocket;
using System.Linq;
using Discord;
using System.IO;
using System.Net.Http;
using System;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteUploadCustomCommand(SocketSlashCommand command)
    {
      string playerName = command.Data.Options.First().Value.ToString();
      Attachment portrait = (Attachment)command.Data.Options.ElementAt(1).Value;

      if(portrait.Filename.Length > 50)
      {
        await command.RespondAsync("Le nom de votre portrait doit être inférieur à 50 caractères.", ephemeral: true);
        return;
      }

      try
      {
        await command.RespondAsync("Demande d'intégration de portrait personnalisé publiée !", ephemeral: true);

        if (!Directory.Exists($"../../../PortraitToValidate/{playerName}"))
          Directory.CreateDirectory($"../../../PortraitToValidate/{playerName}");

        using var client = new HttpClient();
        using var s = await client.GetStreamAsync(portrait.Url);
        using var fs = new FileStream($"../../../PortraitToValidate/{playerName}/{portrait.Filename}", FileMode.OpenOrCreate);
        await s.CopyToAsync(fs);

        IMessageChannel channel = Bot._client.GetChannel(680072044364562532) as IMessageChannel;

        ComponentBuilder builder = new ComponentBuilder()
          .WithButton("Valider", $"send/{playerName}/{portrait.Filename}", ButtonStyle.Success)
          .WithButton("Rejeter", $"delete/{playerName}/{portrait.Filename}", ButtonStyle.Danger);

        await channel.SendFileAsync(fs, portrait.Filename, $"{playerName} - Demande de validation de portrait ({portrait.Filename}) ({command.User.Username})", components: builder.Build());
      }
      catch(Exception e)
      {
        ModuleSystem.Log.Info(e.GetType() + "\n" + e.Message + "\n" + e.StackTrace);
      }
    }
  }
}
