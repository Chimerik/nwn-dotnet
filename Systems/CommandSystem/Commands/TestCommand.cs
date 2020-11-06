using System;
using System.Net;
using System.Web;
using Google.Cloud.Translation.V2;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (player.playerJournal.craftJobCountDown == null)
        {
          player.playerJournal.craftJobCountDown = DateTime.Now.AddMinutes(2);
          JournalEntry journalEntry = new JournalEntry();
          journalEntry.sName = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds(DateTime.Now.AddMinutes(2) - DateTime.Now)}";
          journalEntry.sText = "Vous êtes en train de fabriquer une épée longue en tritanium, cool non ?";
          journalEntry.sTag = "craft_job";
          journalEntry.nPriority = 1;
          journalEntry.nQuestDisplayed = 1;
          PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
        }
        else
        {
          JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid, "craft_job");
          journalEntry.sName = $"Travail artisanal - Terminé !";
          journalEntry.nQuestCompleted = 1;
          journalEntry.nQuestDisplayed = 0;
          PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
          player.playerJournal.craftJobCountDown = null;
        }
      }
    }
    public static String Translate(String word)
    {
      var toLanguage = "en";
      var fromLanguage = "fr";
      var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(word)}";
      var webClient = new WebClient
      {
        Encoding = System.Text.Encoding.UTF8
      };
      var result = webClient.DownloadString(url);
      try
      {
        result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
        return result;
      }
      catch
      {
        return "Error";
      }
    }
  }
}
