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
using Anvil.Services;

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

        if (player.oid.PlayerName == "Chim" || player.oid.PlayerName == "test")
        {
          if (!player.learnableSpells.ContainsKey((int)Spell.LesserRestoration))
            player.learnableSpells.Add((int)Spell.LesserRestoration, new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[(int)Spell.LesserRestoration]));
          if (!player.learnableSpells.ContainsKey((int)Spell.Restoration))
            player.learnableSpells.Add((int)Spell.Restoration, new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[(int)Spell.Restoration]));
          if (!player.learnableSpells.ContainsKey((int)Spell.BullsStrength))
            player.learnableSpells.Add((int)Spell.BullsStrength, new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[(int)Spell.BullsStrength]));
          if (!player.learnableSpells.ContainsKey((int)Spell.Aid))
            player.learnableSpells.Add((int)Spell.Aid, new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[(int)Spell.Aid]));
          if (!player.learnableSpells.ContainsKey((int)Spell.Haste))
            player.learnableSpells.Add((int)Spell.Haste, new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[(int)Spell.Haste]));
          

          if (player.windows.ContainsKey("learnables"))
            ((PlayerSystem.Player.LearnableWindow)player.windows["learnables"]).CreateWindow();
          else
            player.windows.Add("learnables", new PlayerSystem.Player.LearnableWindow(player));

          /*if (player.windows.ContainsKey("quickLoot"))
            ((PlayerSystem.Player.QuickLootWindow)player.windows["quickLoot"]).CreateWindow();
          else
            player.windows.Add("quickLoot", new PlayerSystem.Player.QuickLootWindow(player));

          ScriptCallbackHandle intervalHandle = PlayerSystem.scriptHandleFactory.CreateUniqueHandler(RefreshQuickLootWindow);

          Effect runAction = Effect.RunAction(null, null, intervalHandle, TimeSpan.FromSeconds(2));
          runAction.Tag = "QUICK_LOOT_EFFECT";
          runAction.SubType = EffectSubType.Supernatural;
          runAction = Effect.LinkEffects(runAction, Effect.Icon(EffectIcon.Curse));

          player.oid.ControlledCreature.ApplyEffect(EffectDuration.Permanent, runAction);  */

          //SpellSystem.ApplyCustomEffectToTarget(SpellSystem.frog, player.oid.LoginCreature, TimeSpan.FromSeconds(10));

          //PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, OnTargetSelected, ObjectTypes.All, MouseCursor.Pickup);
        }
      }
    }

    public static ScriptHandleResult RefreshQuickLootWindow(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (!(eventData.EffectTarget is NwCreature oTarget) || !PlayerSystem.Players.TryGetValue(oTarget.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
        return ScriptHandleResult.Handled;

      ((PlayerSystem.Player.QuickLootWindow)player.windows["quickLoot"]).UpdateWindow();

      return ScriptHandleResult.Handled;
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
