﻿using System;
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
        var firstObject = NWScript.GetFirstObjectInArea(NWScript.GetArea(player.oid));
        int i = 1;
        var nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_CREATURE, firstObject);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(nearestObject)))
        {
          if (NWScript.GetIsPC(nearestObject) == 0)
            Utils.DestroyInventory(nearestObject);
          
          NWScript.DestroyObject(nearestObject);
          i++;
          nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_CREATURE, firstObject, i);
        }

        i = 1;
        nearestObject = NWScript.GetNearestObjectByTag("BodyBag", firstObject);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(nearestObject)))
        {
          Utils.DestroyInventory(nearestObject);
          NWScript.DestroyObject(nearestObject);
          i++;
          nearestObject = NWScript.GetNearestObjectByTag("BodyBag", firstObject, i);
        }

        i = 1;
        nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_ITEM, firstObject);

        while (Convert.ToBoolean(NWScript.GetIsObjectValid(nearestObject)))
        {
          NWScript.DestroyObject(nearestObject);
          i++;
          nearestObject = NWScript.GetNearestObject(NWScript.OBJECT_TYPE_ITEM, firstObject, i);
        }

        if (NWScript.GetObjectType(firstObject) == NWScript.OBJECT_TYPE_CREATURE
          || NWScript.GetTag(firstObject) == "BodyBag" || NWScript.GetObjectType(firstObject) == NWScript.OBJECT_TYPE_ITEM)
        {
          if (Convert.ToBoolean(NWScript.GetHasInventory(firstObject)))
            Utils.DestroyInventory(firstObject);
          NWScript.DestroyObject(firstObject);
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
