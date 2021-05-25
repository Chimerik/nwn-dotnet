using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading;
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
using Action = System.Action;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static async void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
      {
        //ctx.oSender.ToNwObject<NwPlayer>().HP = 10;
        //ctx.oSender.ApplyEffect(EffectDuration.Instant, API.Effect.Death());

        if (player.oid.PlayerName == "Chim")
        {
          //SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_FROG", 51, 6);
          PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, OnTargetSelected, ObjectTypes.All, MouseCursor.Pickup);
        }
      }
    }
    private static void OnTargetSelected(ModuleEvents.OnPlayerTarget selection)
    {
      selection.TargetObject.GetLocalVariable<int>("_DURABILITY").Value = -1;

      foreach (API.ItemProperty ip in ((NwItem)selection.TargetObject).ItemProperties.Where(ip => ip.Tag.StartsWith("ENCHANTEMENT")))
      {
        Task waitLoopEnd = NwTask.Run(async () =>
        {
          API.ItemProperty deactivatedIP = ip;
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          ((NwItem)selection.TargetObject).RemoveItemProperty(deactivatedIP);
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          deactivatedIP.Tag += "_INACTIVE";
          ((NwItem)selection.TargetObject).AddItemProperty(deactivatedIP, EffectDuration.Permanent);
        });
      }

      //((NwGameObject)selection.TargetObject).ApplyEffect(EffectDuration.Permanent, API.Effect.Swarm(true, "sim_wraith"));
      //AppearancePlugin.SetOverride(selection.Player, selection.TargetObject, );
      //PlayerPlugin.ApplyLoopingVisualEffectToObject(selection.Player, selection.TargetObject, NWScript.VFX_DUR_PROT_BARKSKIN);
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
