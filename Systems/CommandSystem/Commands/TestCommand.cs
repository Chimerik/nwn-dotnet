using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using System.Web;
using Discord;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
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
          if (player.currentSkillType == SkillType.Skill)
            player.learnableSkills[(Feat)player.currentSkillJob].acquiredPoints = player.learnableSkills[(Feat)player.currentSkillJob].pointsToNextLevel;
          else if (player.currentSkillType == SkillType.Spell)
            player.learnableSpells[player.currentSkillJob].acquiredPoints = player.learnableSpells[player.currentSkillJob].pointsToNextLevel;
          
          //PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, OnTargetSelected, ObjectTypes.All, MouseCursor.Pickup);
        }
      }
    }

    public static void OnTargetSelected(ModuleEvents.OnPlayerTarget selection)
    {
      NWScript.SetObjectVisualTransform(selection.TargetObject, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0.75f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
      //NWScript.SetObjectVisualTransform(selection.TargetObj, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, -10, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
      //NWScript.SetObjectVisualTransform(selection.TargetObj, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Y, -20, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
      NWScript.SetObjectVisualTransform(selection.TargetObject, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, -15, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);

      Task waitLoopEndToRemove = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(2));
        HandleSwing(selection.TargetObject);
      });
    }

    public static void HandleSwing(NwObject swing)
    {
      NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, -NWScript.GetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X), NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
      //NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, -NWScript.GetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X), NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
      //NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Y, -NWScript.GetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Y), NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
      NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, -NWScript.GetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z), NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);

      Task waitLoopEndToRemove = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(2));
        HandleSwing(swing);
      });
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
