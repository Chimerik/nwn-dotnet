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
