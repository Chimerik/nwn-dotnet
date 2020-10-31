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
        TranslationClient client = TranslationClient.Create();
        
        string test = ctx.msg.Replace("!test ", "").Replace('"', ' ');

        TranslationResult result = client.TranslateText(test, LanguageCodes.Galician);

        NWScript.SendMessageToPC(ctx.oSender, result.TranslatedText);

        WebhookSystem.StartSendingAsyncDiscordMessage(test, "AoA Translation test");
        WebhookSystem.StartSendingAsyncDiscordMessage(result.TranslatedText, "AoA Translation test");
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
