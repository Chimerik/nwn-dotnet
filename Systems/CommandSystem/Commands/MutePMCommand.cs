using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteMutePMCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (!PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
        return;

      if (ctx.oTarget != null)
      {
        if (!PlayerSystem.Players.TryGetValue(ctx.oTarget.LoginCreature, out PlayerSystem.Player mutedPlayer))
          return;

        if (!player.mutedList.Contains(mutedPlayer.accountId))
        {
          player.mutedList.Add(mutedPlayer.accountId);
          SaveMutedPlayersToDatabase(player);
          ctx.oSender.SendServerMessage($"Vous bloquez désormais tous les mps de {ctx.oTarget.LoginCreature.Name.ColorString(ColorConstants.White)}. Cette commande ne fonctionne pas sur les Dms.", ColorConstants.Blue);      
        }
        else
        {
          player.mutedList.Remove(mutedPlayer.accountId);
          SaveMutedPlayersToDatabase(player);
          ctx.oSender.SendServerMessage($"Vous ne bloquez plus les mps de {ctx.oTarget.LoginCreature.Name.ColorString(ColorConstants.White)}", ColorConstants.Blue);
        }
      }
      else
      {
        if (!player.mutedList.Contains(0))
        {
          player.mutedList.Add(0);
          SaveMutedPlayersToDatabase(player);
          ctx.oSender.SendServerMessage("Vous bloquez désormais l'affichage global des mps. Vous recevrez cependant toujours ceux des DMs.", ColorConstants.Blue);
        }
        else
        {
          player.mutedList.Remove(0);
          SaveMutedPlayersToDatabase(player);
          ctx.oSender.SendServerMessage("Vous réactivez désormais l'affichage global des mps. Vous ne recevrez cependant pas ceux que vous bloquez individuellement.", ColorConstants.Blue);
        }
      }
    }
    private static async void SaveMutedPlayersToDatabase(PlayerSystem.Player player)
    {
      using (var stream = new MemoryStream())
      {
        await JsonSerializer.SerializeAsync(stream, player.mutedList);
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        string serializedJson = await reader.ReadToEndAsync();

        SqLiteUtils.UpdateQuery("PlayerAccounts",
          new List<string[]>() { new string[] { "mutedPlayers", serializedJson } },
          new List<string[]>() { new string[] { "rowid", player.accountId.ToString() } });
      }
    }
  }
}
