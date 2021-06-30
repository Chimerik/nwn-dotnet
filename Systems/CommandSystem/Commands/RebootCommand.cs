using System;
using System.Collections.Generic;
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

        SqLiteUtils.UpdateQuery("moduleInfo",
        new Dictionary<string, string>() { { "year", NwDateTime.Now.Year.ToString() }, { "month", NwDateTime.Now.Month.ToString() }, { "day", NwDateTime.Now.DayInTenday.ToString() }, { "hour", NwDateTime.Now.Hour.ToString() }, { "minute", NwDateTime.Now.Minute.ToString() }, { "second", NwDateTime.Now.Second.ToString() } },
        new Dictionary<string, string>() { { "rowid", "1" } });

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
