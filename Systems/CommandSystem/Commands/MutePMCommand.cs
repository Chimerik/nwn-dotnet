using System.Collections.Generic;

using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static async void ExecuteMutePMCommand(ChatSystem.Context ctx, Options.Result options)
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

          bool queryResult = await SqLiteUtils.InsertQueryAsync("playerMutedPM",
          new List<string[]>() {
            new string[] { "accountId", player.accountId.ToString() },
            new string[] { "mutedAccountId", mutedPlayer.accountId.ToString()} });

          player.HandleAsyncQueryFeedback(queryResult, $"Vous bloquez désormais tous les mps de {ctx.oTarget.LoginCreature.Name.ColorString(ColorConstants.White)}. Cette commande ne fonctionne pas sur les Dms.", "Erreur technique - Les MPS ne seront pas bloqués.");         
        }
        else
        {
          player.mutedList.Remove(mutedPlayer.accountId);

          SqLiteUtils.DeletionQuery("playerMutedPM",
            new Dictionary<string, string>() { { "accountId", player.accountId.ToString() }, { "mutedAccountId", mutedPlayer.accountId.ToString() } });

          ctx.oSender.SendServerMessage($"Vous ne bloquez plus les mps de {ctx.oTarget.LoginCreature.Name.ColorString(ColorConstants.White)}", ColorConstants.Blue);
        }
      }
      else
      {
        if (!player.mutedList.Contains(0))
        {
          player.mutedList.Add(0);

          bool queryResult = await SqLiteUtils.InsertQueryAsync("playerMutedPM",
          new List<string[]>() {
            new string[] { "accountId", player.accountId.ToString() },
            new string[] { "mutedAccountId", "0" } });

          player.HandleAsyncQueryFeedback(queryResult, $"Vous bloquez désormais l'affichage global des mps. Vous recevrez cependant toujours ceux des DMs.", "Erreur technique - Les MPS ne seront pas bloqués.");
        }
        else
        {
          player.mutedList.Remove(0);

          SqLiteUtils.DeletionQuery("playerMutedPM",
            new Dictionary<string, string>() { { "accountId", player.accountId.ToString() }, { "mutedAccountId", "0" } });

          ctx.oSender.SendServerMessage("Vous réactivez désormais l'affichage global des mps. Vous ne recevrez cependant pas ceux que vous bloqué individuellement.", ColorConstants.Blue);
        }
      }
    }
  }
}
