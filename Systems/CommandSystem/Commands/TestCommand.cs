using System;
using System.Net;
using System.Web;
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
        //VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetNearestObjectByTag("statuetest", player.oid), VisibilityPlugin.NWNX_VISIBILITY_ALWAYS_VISIBLE);
        //ObjectPlugin.SetPosition(NWScript.GetNearestObjectByTag("statuetest", player.oid), NWScript.GetPosition(player.oid));
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
