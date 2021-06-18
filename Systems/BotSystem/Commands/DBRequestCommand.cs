﻿using Discord.Commands;
using System;
using System.Threading.Tasks;
using NWN.API;
using NWN.Services;
using NWNX.API;
using NWN.Core.NWNX;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDBRequestCommand(SocketCommandContext context, string request)
    {
      await NwTask.SwitchToMainThread();

      if (DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id) != "admin")
      {
        await context.Channel.SendMessageAsync("Noooon, vous n'êtes pas la maaaaaître ! Le maaaaître est bien plus poli, d'habitude !");
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, request);
      NWScript.SqlStep(query);
      string error = NWScript.SqlGetError(query);

      if (error == "")
        await context.Channel.SendMessageAsync("Requête correctement exécutée.");
      else
        await context.Channel.SendMessageAsync($"SQL ERROR : {error}");
    }
  }
}