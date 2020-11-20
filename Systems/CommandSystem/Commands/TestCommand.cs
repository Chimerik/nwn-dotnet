using System;
using System.Net;
using System.Web;
using NWN.Core;
using NWN.Core.NWNX;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        player.learnableSkills.Clear();
        CreaturePlugin.RemoveFeat(player.oid, 1145);
        CreaturePlugin.RemoveFeat(player.oid, 1146);
        CreaturePlugin.RemoveFeat(player.oid, 1147);
        CreaturePlugin.RemoveFeat(player.oid, 1148);
        CreaturePlugin.RemoveFeat(player.oid, 1149);
        PlayerSystem.InitializeNewPlayerLearnableSkills(player);
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
