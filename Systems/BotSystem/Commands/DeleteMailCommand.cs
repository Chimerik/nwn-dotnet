﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteDeleteMailCommand(SocketCommandContext context, string mailId, string characterName)
    {
      int result = await DiscordUtils.CheckPlayerCredentialsFromDiscord(context, characterName);

      if (result <= 0)
      {
        await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
        return;
      }

      SqLiteUtils.DeletionQuery("messenger",
        new Dictionary<string, string>() { { "characterId", result.ToString() }, { "ROWID", mailId } });

      await context.Channel.SendMessageAsync("Message supprimé");
    }
  }
}
