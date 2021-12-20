using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.Services;

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
          JournalEntry journalEntry = new JournalEntry();
          journalEntry.Name = "REBOOT SERVEUR - 30";
          journalEntry.Text = "Attention, le serveur reboot bientôt. Accrochez bien vos ceintures.\n" +
            "Non pas que vous ayez grand chose à faire, votre personnage est automatiquement sauvegardé et le module sera de retour dans moins d'une minute.";
          journalEntry.QuestTag = "reboot";
          journalEntry.Priority = 1;
          journalEntry.QuestCompleted = false;
          journalEntry.QuestDisplayed = true;
          player.oid.AddCustomJournalEntry(journalEntry);

          //player.rebootUpdate(29);
        }
      }

      Task Reboot = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(30));

        SqLiteUtils.UpdateQuery("moduleInfo",
        new List<string[]>() { new string[] { "year", NwDateTime.Now.Year.ToString() }, new string[] { "month", NwDateTime.Now.Month.ToString() }, new string[] { "day", NwDateTime.Now.DayInTenday.ToString() }, new string[] { "hour", NwDateTime.Now.Hour.ToString() }, new string[] { "minute", NwDateTime.Now.Minute.ToString() }, new string[] { "second", NwDateTime.Now.Second.ToString() } },
        new List<string[]>() { new string[] { "rowid", "1" } });

        Utils.BootAllPC();

        Task waitServerEmpty = NwTask.Run(async () =>
        {
          await NwTask.WaitUntil(() => NwModule.Instance.Players.Count() < 1);
          await NwTask.Delay(TimeSpan.FromSeconds(1));
          NwServer.Instance.ShutdownServer();
        });
      });
    }
  }
}
