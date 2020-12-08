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
        uint oItem = NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid);
        NWScript.AddItemProperty(NWScript.DURATION_TYPE_PERMANENT, NWScript.ItemPropertyDamageVulnerability(NWScript.IP_CONST_DAMAGETYPE_FIRE, NWScript.IP_CONST_DAMAGEVULNERABILITY_50_PERCENT), oItem);
        /*foreach (ItemProperty ip in Blueprint.GetCraftItemProperties(CollectSystem.GetMineralTypeFromName("Pyerite"), ItemSystem.GetItemCategory(NWScript.GetBaseItemType(oItem))))
        {
          NWScript.AddItemProperty(NWScript.DURATION_TYPE_PERMANENT, ip, oItem);
        }*/

        //player.craftJob = new CraftJob(58, "Pyerite", 10, player);

        //NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_START_NEW_CHAR"))));
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
