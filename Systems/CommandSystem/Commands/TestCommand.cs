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
        QuickBarSlot qbs = PlayerPlugin.GetQuickBarSlot(player.oid, 0);
        NWScript.SendMessageToPC(player.oid, $"objectType = {qbs.nObjectType}");
        NWScript.SendMessageToPC(player.oid, $"resREf = {qbs.sResRef}");
        NWScript.SendMessageToPC(player.oid, $"Tooltip = {qbs.sToolTip}");
        NWScript.SendMessageToPC(player.oid, $"param1 = {qbs.nINTParam1}");

        SpecialAbility test;
        test.id = NWScript.SPELL_FIREBALL;
        test.ready = 1;
        test.level = 1;
        CreaturePlugin.AddSpecialAbility(player.oid, test);
        /*CreaturePlugin.AddKnownSpell(player.oid, NWScript.CLASS_TYPE_BARBARIAN, 1, NWScript.SPELL_FIREBALL);
        CreaturePlugin.AddKnownSpell(player.oid, NWScript.CLASS_TYPE_WIZARD, 1, NWScript.SPELL_FLAME_STRIKE);*/
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
