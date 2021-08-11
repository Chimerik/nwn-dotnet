using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using Utils;

namespace BotSystem
{
  public static partial class BotCommand
  {
    public static async Task ExecuteDeleteMailCommand(SocketCommandContext context, string mailId, string characterName)
    {
      await NwTask.SwitchToMainThread();
      int result = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, characterName);

      if (result <= 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      if (SqLiteUtils.DeletionQuery("messenger",
        new Dictionary<string, string>() { { "characterId", result.ToString() }, { "ROWID", mailId } }))
        await context.Channel.SendMessageAsync("Message supprimé");
      else
        await context.Channel.SendMessageAsync($"Erreur technique - le message n'a pas pu être supprimé.");
    }
  }
}
