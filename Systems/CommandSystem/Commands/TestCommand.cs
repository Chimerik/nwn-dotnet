﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
      {
        //ctx.oSender.ToNwObject<NwPlayer>().HP = 10;
        //ctx.oSender.ApplyEffect(EffectDuration.Instant, Effect.Death());

        if (player.oid.PlayerName == "Chim")
        {
          /*Effect eff = Effect.DamageIncrease(10, DamageType.Acid);
          Effect link = Effect.LinkEffects(eff, Effect.DamageIncrease(8, DamageType.Acid));
          link = Effect.LinkEffects(link, Effect.DamageIncrease(8, DamageType.Electrical));
          link = Effect.LinkEffects(link, Effect.DamageIncrease(6, DamageType.Electrical));

          player.oid.ControlledCreature.ApplyEffect(EffectDuration.Temporary, link, TimeSpan.FromSeconds(10));

          foreach (var effectType in player.oid.ControlledCreature.ActiveEffects.Where(e => e.EffectType == EffectType.DamageIncrease)
            .GroupBy(e => e.IntParams.ElementAt(1)))
          {
            Effect maxEffect = effectType.OrderByDescending(e => e.IntParams.ElementAt(0)).FirstOrDefault();
            Log.Info($"found : {(DamageType)maxEffect.IntParams.ElementAt(1)} max value : {maxEffect.IntParams.ElementAt(0)}");
          }*/

            /*GC.SuppressFinalize(eff);
            player.oid.ControlledCreature.ApplyEffect(EffectDuration.Temporary, eff, TimeSpan.FromSeconds(10));

            var test = EffectPlugin.UnpackEffect(eff);
            Log.Info($"numIntegers : {test.nNumIntegers} - n0 : {test.nParam0} - n1 : {test.nParam1} - n2 : {test.nParam2} -");
            */
            //SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_FROG", 51, 6);
            PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, OnTargetSelected, ObjectTypes.All, MouseCursor.Pickup);
        }
      }
    }
    
    private static void OnTargetSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled)
        return;

      Log.Info($"type : {selection.TargetObject.GetType()}");

      if (!(selection.TargetObject is NwItem item))
        return;

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
