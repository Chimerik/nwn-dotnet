using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRebootCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender) == 1)
      {
        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          NWScript.FloatingTextStringOnCreature("Attention - Le serveur va redémarrer dans 30 secondes.", PlayerListEntry.Key, 0);
          PlayerListEntry.Value.playerJournal.rebootCountDown = DateTime.Now.AddSeconds(30);
          
          JournalEntry journalEntry = new JournalEntry();
          journalEntry.sName = $"REBOOT SERVEUR - {NWN.Utils.StripTimeSpanMilliseconds((TimeSpan)(PlayerListEntry.Value.playerJournal.rebootCountDown - DateTime.Now))}";
          journalEntry.sText = "Attention, le serveur reboot bientôt. Accrochez bien vos ceintures.\n" +
            "Non pas que vous ayez grand chose à faire, votre personnage est automatiquement sauvegardé et le module sera de retour dans moins d'une minute.";
          journalEntry.sTag = "reboot";
          journalEntry.nPriority = 1;
          journalEntry.nQuestCompleted = 0;
          journalEntry.nQuestDisplayed = 1;
          PlayerPlugin.AddCustomJournalEntry(PlayerListEntry.Key, journalEntry);

          PlayerListEntry.Value.rebootUpdate();
        }

        AdminPlugin.SetPlayerPassword("REBOOT");
        NWScript.AssignCommand(NWScript.GetModule(), () => NWScript.DelayCommand(30.0f, () => NWN.Utils.BootAllPC()));
      }
    }
  }
}
