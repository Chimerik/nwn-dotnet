using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Web;
using Discord;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        if(NWScript.GetPCPlayerName(player.oid) == "Chim")
        {
          //PlayerPlugin.PlaySound(player.oid, "song_test");
          Action<uint, Vector3> callback = (uint oTarget, Vector3 position) =>
          {
            // HP TEST
            int improvedConst = CreaturePlugin.GetHighestLevelOfFeat(oTarget, (int)Feat.ImprovedConstitution);
            if (improvedConst == (int)Feat.Invalid)
              improvedConst = 0;
            else
              improvedConst = Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", improvedConst));

            //NWScript.SendMessageToPC(player.oid, $"pv : {Int32.Parse(NWScript.Get2DAString("classes", "HitDie", 43)) + (1 + 3 * ((NWScript.GetAbilityScore(oTarget, NWScript.ABILITY_CONSTITUTION, 1) + improvedConst - 10) / 2 + CreaturePlugin.GetKnowsFeat(oTarget, (int)Feat.Toughness))) * Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(oTarget, (int)Feat.ImprovedHealth)))}");

            CreaturePlugin.SetMaxHitPointsByLevel(oTarget, 1, Int32.Parse(NWScript.Get2DAString("classes", "HitDie", 43))
              + (1 + 3 * ((NWScript.GetAbilityScore(oTarget, NWScript.ABILITY_CONSTITUTION, 1)
              + improvedConst - 10) / 2
              + CreaturePlugin.GetKnowsFeat(oTarget, (int)Feat.Toughness))) * Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(oTarget, (int)Feat.ImprovedHealth))));
          };

          player.targetEvent = TargetEvent.LootSaverTarget;
          player.SelectTarget(callback);
          
          // TEST BOT

          //string test = Bot._client.GetUser(232218662080086017).Mention;//.SendMessageAsync("BOT TEST !");

          //(Bot._client.GetUser(232218662080086017).GetOrCreateDMChannelAsync() as IDMChannel).SendMessageAsync("BOT TEST !");
          //(Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"BOT TEST !");

          //Bot._client.GetUser(232218662080086017).SendMessageAsync("BOT TEST !");
          //(Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} MENTION TEST EVERYONE!");
          //(Bot._client.GetChannel(786218144296468481) as IMessageChannel).SendMessageAsync($"{test} MESSAGE TEST !");


          // TEST DEATH & RESPAWN

          //NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectDamage(9999), player.oid);
          //Location loc = NWScript.GetLocation(player.oid);
          //NWScript.DelayCommand(5.0f, () => NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(loc)));
          //NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_START_NEW_CHAR"))));
        }
      }
    }

   /* public static String Translate(String word)
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
    }*/
  }
}
