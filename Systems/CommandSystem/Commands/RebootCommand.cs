using System;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  class Reboot
  {
    public Reboot()
    {
      NwServer.Instance.PlayerPassword = "REBOOT";

      foreach (NwPlayer oPC in NwModule.Instance.Players)
      {
        if (PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player player))
        {
          API.JournalEntry journalEntry = new API.JournalEntry();
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
      }

      Task Reboot = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(30));

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE moduleInfo SET year = @year, month = @month, " +
        $"day = @day, hour = @hour, minute = @minute, second = @second where rowid = 1");
        NWScript.SqlBindInt(query, "@year", NwDateTime.Now.Year);
        NWScript.SqlBindInt(query, "@month", NwDateTime.Now.Month);
        NWScript.SqlBindInt(query, "@day", NwDateTime.Now.DayInTenday);
        NWScript.SqlBindInt(query, "@hour", NwDateTime.Now.Hour);
        NWScript.SqlBindInt(query, "@minute", NwDateTime.Now.Minute);
        NWScript.SqlBindInt(query, "@second", NwDateTime.Now.Second);
        NWScript.SqlStep(query);

        await NwModule.Instance.AddActionToQueue(() => Utils.BootAllPC());

        Task waitServerEmpty = NwTask.Run(async () =>
        {
          await NwTask.WaitUntil(() => NwModule.Instance.Players.Count() < 1);
          NwServer.Instance.ShutdownServer();
        });
      });
    }
  }
}
