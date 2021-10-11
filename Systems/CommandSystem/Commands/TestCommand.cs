using System;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core.NWNX;
using System.Numerics;
using System.Threading.Tasks;
using NWN.Core;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using NWN.Systems.Alchemy;
using Newtonsoft.Json;

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
          //player.CreateBodyAppearanceWindow();
          //player.CreateItemAppearanceWindow(player.oid.ControlledCreature.GetItemInSlot(InventorySlot.Chest));
          //player.CreatePortraitDemoWindow();
          //player.CreateItemColorsWindow(player.oid.ControlledCreature.GetItemInSlot(InventorySlot.Chest)); 
          //player.CreateLearnablesWindow();

          //player.CreateQuickLootWindow(player.oid.ControlledCreature.Area.FindObjectsOfTypeInArea<NwItem>().FirstOrDefault(i => i.Possessor is null && i.DistanceSquared(player.oid.ControlledCreature) < 25));

          //ChatSystem.chatService.SendMessage(Anvil.Services.ChatChannel.PlayerTell, "test", player.oid.ControlledCreature, player.oid);
          //player.CreateChatWindow();

          //SpellSystem.ApplyCustomEffectToTarget(SpellSystem.frog, player.oid.LoginCreature, TimeSpan.FromSeconds(10));

          //PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, OnTargetSelected, ObjectTypes.All, MouseCursor.Pickup);
        }
      }
    }

    private static void OnTargetSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled)
        return;

      Log.Info($"type : {selection.TargetObject.GetType()}");

      if (!(selection.TargetObject is NwCreature creature))
        return;

      foreach (Effect eff in creature.ActiveEffects.Where(e => e.Tag == "_FREEZE_EFFECT"))
        creature.RemoveEffect(eff);

      int anim = creature.GetObjectVariable<LocalVariableInt>("anim").Value;
      anim += 1;

      if (anim == 21)
        anim = 100;
      if (anim == 117)
        anim = 0;

      creature.GetObjectVariable<LocalVariableInt>("anim").Value = anim;

      creature.PlayAnimation((Animation)anim, 1, false, TimeSpan.FromDays(365));
      /*creature.PlayAnimation(Animation.LoopingMeditate, 1, false, TimeSpan.FromDays(365));

      Task createWP = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(3));
        Effect test = Effect.VisualEffect(VfxType.DurFreezeAnimation);
        creature.ApplyEffect(EffectDuration.Permanent, test);
      });*/

      Log.Info($"{creature.Name} playing animation {(Animation)anim}");
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
