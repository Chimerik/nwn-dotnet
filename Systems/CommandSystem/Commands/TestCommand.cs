using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using System.Web;
using Discord;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        //ctx.oSender.ToNwObject<NwPlayer>().HP = 10;
        //ctx.oSender.ToNwObject<NwPlayer>().ApplyEffect(API.EffectDuration.Instant, API.Effect.Death());

        if (NWScript.GetPCPlayerName(player.oid) == "Chim")
        {
          player.oid.SendServerMessage($"modificateur de dex : {player.oid.GetAbilityModifier(Ability.Dexterity)}");

          PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, SelectObject, ObjectTypes.All, MouseCursor.Kill);
        }
      }
    }
    private static void SelectObject(CursorTargetData selection)
    {
      NWScript.SetObjectVisualTransform(selection.TargetObj, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, 10, 
        NWScript.OBJECT_VISUAL_TRANSFORM_LERP_LINEAR, 10);
      NWScript.SetObjectVisualTransform(selection.TargetObj, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 10,
        NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 10);
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
