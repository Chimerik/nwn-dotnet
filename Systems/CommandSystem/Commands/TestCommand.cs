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
          /*Action<uint, Vector3> callback = (uint oTarget, Vector3 position) =>
          {

          };

          player.targetEvent = TargetEvent.LootSaverTarget;
          player.SelectTarget(callback);*/

          float facing = NWScript.GetFacing(player.oid);
          /*float movx = 0;
          float movy = 0;

          if (facing == 0)
            facing = 0.0000001f;

          if (facing > 0 && facing <= 45)
          {
            movy = (float)Math.Sin(facing) * 1;
            movx = (float)Math.Tan(facing) / movy;
          }
          else if (facing > 45 && facing <= 90)
          {
            facing = 90 - facing;
            movy = (float)Math.Sin(facing) * 1;
            movx = (float)Math.Tan(facing) / movy;
          }
          else if (facing > 270 && facing <= 360)
            facing = 360 - facing;

          */

          float movx = 0;// = (float)Math.Sin(Math.PI / 180 * facing) * 1;
          float movy = 0;// = movx / (float)Math.Tan(Math.PI / 180 * facing);

          if (facing >= 0 && facing <= 90)
          {
            movx = -(90 - facing) / 90 * 1;
            movy = -(movx + 1);
          }
          else if(facing > 90 && facing <= 180)
          {
            movy = -(90 - (facing - 90)) / 90 * 1;
            movx = (movy + 1);
            
          }
          else if (facing > 180 && facing <= 270)
          {
            movx = (90 - (facing - 180)) / 90 * 1;
            movy = (movx - 1);
          }
          else if (facing > 270 && facing <= 360)
          {
            movy = (90 - facing - 270) / 90 * 1;
            movx = (1 - movy);
          }

          NWScript.SendMessageToPC(player.oid, $"facing : {facing}");
          NWScript.SendMessageToPC(player.oid, $"X : {movx}");
          NWScript.SendMessageToPC(player.oid, $"Y : {movy}");

          NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X) + movx);
          NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) + movy);
          /*
          var α = NWScript.GetFacing(player.oid);
          var αTemp = α % Math.PI / 2; //We put it in the first quarter, it will not change the amplitude.
          //var amplitude = R / Math.Cos(αTemp);

          var X = Math.Cos(α);
          var Y = Math.Sin(α);

          NWScript.SendMessageToPC(player.oid, $"X : {X}");
          NWScript.SendMessageToPC(player.oid, $"Y : {Y}");

          NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X) + (float)X);
          NWScript.SetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, NWScript.GetObjectVisualTransform(player.oid, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) + (float)Y);
*/
          //NWScript.SendMessageToPC(NWScript.GetFirstPC(), $"role : {Bot.discordServer.role.EveryoneRole.Mention}");


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
