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
        CreaturePlugin.SetMovementRate(player.oid, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_PC);
        /*if (player.currentHP != 1)
        {
          player.BoulderBlock();
          player.currentHP = 1;
        }
        else
        {
          player.BoulderUnblock();
          player.currentHP = NWScript.GetMaxHitPoints(player.oid);
        }*/
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
