using Discord.Commands;
using System;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.Services;
using System.Linq;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteRebootCommand(SocketCommandContext context)
    {
      Utils.LogMessageToDMs($"Reboot command used by {context.User.Username}");

      string rank = await DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id);

      if (rank != "admin")
      {
        await context.Channel.SendMessageAsync("Noooon, vous n'êtes pas la maaaaaître ! Le maaaaître est bien plus poli, d'habitude !");
        return;
      }

      await NwTask.SwitchToMainThread();

      foreach (NwPlayer oPC in NwModule.Instance.Players)
      {
        if (!PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player player))
          continue;

        JournalEntry journalEntry = new JournalEntry();
        journalEntry.Name = "REBOOT SERVEUR - 30";
        journalEntry.Text = "Attention, le serveur reboot bientôt. Accrochez bien vos ceintures.\n" +
          "Non pas que vous ayez grand chose à faire, votre personnage est automatiquement sauvegardé et le module sera de retour dans moins d'une minute.";
        journalEntry.QuestTag = "reboot";
        journalEntry.Priority = 1;
        journalEntry.QuestCompleted = false;
        journalEntry.QuestDisplayed = true;
        player.oid.AddCustomJournalEntry(journalEntry);

        player.rebootUpdate(29);
      }

      Task Reboot = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(30));

        SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "year", NwDateTime.Now.Year.ToString() }, new string[] { "month", NwDateTime.Now.Month.ToString() }, new string[] { "day", NwDateTime.Now.DayInTenday.ToString() }, new string[] { "hour", NwDateTime.Now.Hour.ToString() }, new string[] { "minute", NwDateTime.Now.Minute.ToString() }, new string[] { "second", NwDateTime.Now.Second.ToString() } },
          new List<string[]>() { new string[] { "rowid", "1" } });

        await NwModule.Instance.AddActionToQueue(() => Utils.BootAllPC());

        Task waitServerEmpty = NwTask.Run(async () =>
        {
          await NwTask.WaitUntil(() => NwModule.Instance.Players.Count() < 1);
          NwServer.Instance.ShutdownServer();
        });
      });

      await context.Channel.SendMessageAsync("Reboot effectif dans 30 secondes.");
    }
  }
}
