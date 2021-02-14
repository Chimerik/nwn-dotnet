using System;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWNX.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRebootCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender) == 1 || NWScript.GetPCPlayerName(ctx.oSender) == "Chim")
      {
        AdminPlugin.SetPlayerPassword("REBOOT");

        foreach (NwPlayer oPC in NwModule.Instance.Players)
        {
          oPC.FloatingTextString("Attention - Le serveur va redémarrer dans 30 secondes.", false);

          if (PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
          {
            player.playerJournal.rebootCountDown = DateTime.Now.AddSeconds(30);

            JournalEntry journalEntry = new JournalEntry();
            journalEntry.sName = $"REBOOT SERVEUR - {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.rebootCountDown - DateTime.Now))}";
            journalEntry.sText = "Attention, le serveur reboot bientôt. Accrochez bien vos ceintures.\n" +
              "Non pas que vous ayez grand chose à faire, votre personnage est automatiquement sauvegardé et le module sera de retour dans moins d'une minute.";
            journalEntry.sTag = "reboot";
            journalEntry.nPriority = 1;
            journalEntry.nQuestCompleted = 0;
            journalEntry.nQuestDisplayed = 1;
            PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);

            player.rebootUpdate();
          }
        }

        Task Reboot = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(30));

          var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE moduleInfo SET year = @year, month = @month, " +
          $"second = @second");
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
      }
    }
  }
}
