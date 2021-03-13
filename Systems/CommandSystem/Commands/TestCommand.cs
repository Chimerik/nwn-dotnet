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
          player.oid.SendServerMessage($"modificateur de dex : {player.oid.GetAbilityModifier(Ability.Dexterity)}");
          
        }
      }
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
