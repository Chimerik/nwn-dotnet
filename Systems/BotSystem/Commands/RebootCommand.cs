using Discord.Commands;
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
    public static async Task ExecuteRebootCommand(SocketCommandContext context)
    {
      await NwTask.SwitchToMainThread();

      if (DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id) != "admin")
      {
        await context.Channel.SendMessageAsync("Noooon, vous n'êtes pas la maaaaaître ! Le maaaaître est bien plus poli, d'habitude !");
        return;
      }

      Administration.PlayerPassword = "REBOOT";

      foreach (NwPlayer oPC in NwModule.Instance.Players)
      {
        if (PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        {
          JournalEntry journalEntry = new JournalEntry();
          journalEntry.sName = "REBOOT SERVEUR - 30";
          journalEntry.sText = "Attention, le serveur reboot bientôt. Accrochez bien vos ceintures.\n" +
            "Non pas que vous ayez grand chose à faire, votre personnage est automatiquement sauvegardé et le module sera de retour dans moins d'une minute.";
          journalEntry.sTag = "reboot";
          journalEntry.nPriority = 1;
          journalEntry.nQuestCompleted = 0;
          journalEntry.nQuestDisplayed = 1;
          PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);

          player.rebootUpdate(29);
        }
      }

      Task Reboot = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(30));

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE moduleInfo SET year = @year, month = @month, " +
          $"day = @day, hour = @hour, minute = @minute, second = @second");
        NWScript.SqlBindInt(query, "@year", NwDateTime.Now.Year);
        NWScript.SqlBindInt(query, "@month", NwDateTime.Now.Month);
        NWScript.SqlBindInt(query, "@day", NwDateTime.Now.DayInTenday);
        NWScript.SqlBindInt(query, "@hour", NwDateTime.Now.Hour);
        NWScript.SqlBindInt(query, "@minute", NwDateTime.Now.Minute);
        NWScript.SqlBindInt(query, "@second", NwDateTime.Now.Second);
        NWScript.SqlStep(query);

        await NwModule.Instance.AddActionToQueue(() => NWN.Utils.BootAllPC());

        Administration.ShutdownServer();
        return 20;
      });

      await context.Channel.SendMessageAsync("Reboot effectif dans 30 secondes.");
    }
  }
}
