using System;
using System.Net;
using System.Numerics;
using System.Web;
using Discord;
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
        if(NWScript.GetPCPlayerName(player.oid) == "Chim")
        {
          Action<uint, Vector3> callback = (uint oTarget, Vector3 position) =>
          {
            Effect eRay = NWScript.EffectBeam(NWScript.VFX_BEAM_DISINTEGRATE, NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid), 1, 0, 3);
            eRay = NWScript.TagEffect(eRay, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eRay, oTarget, 10.0f);
          };

          player.targetEvent = TargetEvent.LootSaverTarget;
          player.SelectTarget(callback);

          //string test = Bot._client.GetUser(232218662080086017).Mention;//.SendMessageAsync("BOT TEST !");

          //(Bot._client.GetUser(232218662080086017).GetOrCreateDMChannelAsync() as IDMChannel).SendMessageAsync("BOT TEST !");
          //(Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"BOT TEST !");

          //Bot._client.GetUser(232218662080086017).SendMessageAsync("BOT TEST !");
          //(Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} MENTION TEST EVERYONE!");
          //(Bot._client.GetChannel(786218144296468481) as IMessageChannel).SendMessageAsync($"{test} MESSAGE TEST !");



          //NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectDamage(9999), player.oid);
          //Location loc = NWScript.GetLocation(player.oid);
          //NWScript.DelayCommand(5.0f, () => NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(loc)));
          //NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_START_NEW_CHAR"))));
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
