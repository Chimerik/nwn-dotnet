using System;
using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
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
      if (!(selection.TargetObject is NwItem item))
        return;

      item.AddItemProperty(API.ItemProperty.ACBonusVsDmgType((IPDamageType)5, 80), EffectDuration.Temporary, TimeSpan.FromSeconds(10));
      //item.AddItemProperty(API.ItemProperty.Custom(NWScript.ITEM_PROPERTY_AC_BONUS, -1, 80), EffectDuration.Temporary, TimeSpan.FromSeconds(10));

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
