﻿using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteGetMailCommand(SocketCommandContext context, int mailId, string characterName)
    {
      await NwTask.SwitchToMainThread();
      int result = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, characterName);

      if (result <= 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT senderName, title, sentDate, message from messenger where characterId = @characterId and ROWID = @mailId");
      NWScript.SqlBindInt(query, "@characterId", result);
      NWScript.SqlBindInt(query, "@mailId", mailId);

      if(NWScript.SqlStep(query) == 0)
      {
        await context.Channel.SendMessageAsync($"Le personnage indiqué n'a pas reçu de message dont le numéro correspond à {mailId}.");
        return;
      }

      var update = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE messenger SET read = 1 where rowid = @messageId");
      NWScript.SqlBindInt(update, "@messageId", mailId);
      NWScript.SqlStep(update);

      await context.Channel.SendMessageAsync($"De {NWScript.SqlGetString(query, 0)}");
      await context.Channel.SendMessageAsync($"Envoyé le {NWScript.SqlGetString(query, 2)} :");
      await context.Channel.SendMessageAsync($"{NWScript.SqlGetString(query, 1)}");
      await context.Channel.SendMessageAsync($"{NWScript.SqlGetString(query, 3)}");
    }
  }
}