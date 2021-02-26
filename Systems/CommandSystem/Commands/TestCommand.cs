using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Web;
using Discord;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static Dictionary<int, API.ItemProperty[]> enchantementCategories = new Dictionary<int, API.ItemProperty[]>()
    {
      {841, new API.ItemProperty[] { API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidGoblinoid, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Animal, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidReptilian, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Vermin, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidGoblinoid, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Animal, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidReptilian, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Vermin, 1) } },
    };
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        //ctx.oSender.ToNwObject<NwPlayer>().HP = 10;
        //ctx.oSender.ToNwObject<NwPlayer>().ApplyEffect(API.EffectDuration.Instant, API.Effect.Death());

        if (NWScript.GetPCPlayerName(player.oid) == "Chim")
        {
          Action<uint, Vector3> callback = (uint oTarget, Vector3 position) =>
          {
            //NWScript.SetLocalInt(oTarget, "_AVAILABLE_ENCHANTEMENT_SLOT", 2);

            // HP TEST
            /*int improvedConst = CreaturePlugin.GetHighestLevelOfFeat(oTarget, (int)Feat.ImprovedConstitution);
            if (improvedConst == (int)Feat.Invalid)
              improvedConst = 0;
            else
              improvedConst = Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", improvedConst));

            //NWScript.SendMessageToPC(player.oid, $"pv : {Int32.Parse(NWScript.Get2DAString("classes", "HitDie", 43)) + (1 + 3 * ((NWScript.GetAbilityScore(oTarget, NWScript.ABILITY_CONSTITUTION, 1) + improvedConst - 10) / 2 + CreaturePlugin.GetKnowsFeat(oTarget, (int)Feat.Toughness))) * Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(oTarget, (int)Feat.ImprovedHealth)))}");

            CreaturePlugin.SetMaxHitPointsByLevel(oTarget, 1, Int32.Parse(NWScript.Get2DAString("classes", "HitDie", 43))
              + (1 + 3 * ((NWScript.GetAbilityScore(oTarget, NWScript.ABILITY_CONSTITUTION, 1)
              + improvedConst - 10) / 2
              + CreaturePlugin.GetKnowsFeat(oTarget, (int)Feat.Toughness))) * Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(oTarget, (int)Feat.ImprovedHealth))));
          */
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
    private static void DrawEnchantementChoicePage(PlayerSystem.Player player, string itemName, int spellId)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quel enchantement souhaitez-vous appliquer sur votre {itemName} ?"
      };

      foreach(API.ItemProperty ip in enchantementCategories[spellId])
        player.menu.choices.Add(($"{NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("itempropdef", "Name", (int)ip.PropertyType)))} - {NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString(NWScript.Get2DAString("itempropdef", "SubTypeResRef", (int)ip.PropertyType), "Name", (int)ip.SubType)))}", () => HandleEnchantementChoice(player, spellId, ip)));
      
      //player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }

    private static void HandleEnchantementChoice(PlayerSystem.Player player, int spellId, API.ItemProperty ip)
    {
      player.oid.SendServerMessage($"ip choisie : {ip.ToString()}");
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
