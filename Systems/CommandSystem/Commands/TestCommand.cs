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
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        //ctx.oSender.ToNwObject<NwPlayer>().HP = 10;
        ctx.oSender.ApplyEffect(EffectDuration.Instant, API.Effect.Death());

        if (NWScript.GetPCPlayerName(player.oid) == "Chim")
        {
          /*player.menu.Clear();

          player.menu.titleLines = new List<string>() {
          "Sélectionnez votre malus !",
          };

          int random = NwRandom.Roll(Utils.random, 20);

          player.menu.choices.Add((
            arenaMalusDictionary[random],
            () => ApplyArenaMalus(player, random)
          ));

          player.menu.Draw();
          RandomizeMalusSelection(player);*/

          //PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, OnTargetSelected, ObjectTypes.All, MouseCursor.Pickup);
        }
      }
    }
    private static async void RandomizeMalusSelection(PlayerSystem.Player player)
    {
      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task malusSelected = NwTask.WaitUntil(() => player.oid.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").HasValue, tokenSource.Token);
      Task waitingForSelection = NwTask.Delay(TimeSpan.FromSeconds(0.2), tokenSource.Token);

      await NwTask.WhenAny(malusSelected, waitingForSelection);
      tokenSource.Cancel();

      if (malusSelected.IsCompletedSuccessfully)
      {
        player.oid.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").Delete();
        return;
      }

      int random = NwRandom.Roll(Utils.random, 20);

      player.menu.choices.Clear();

      player.menu.choices.Add((
        arenaMalusDictionary[random],
        () => ApplyArenaMalus(player, random)
      ));

      player.menu.DrawText();

      RandomizeMalusSelection(player);
    }
    private static void ApplyArenaMalus(PlayerSystem.Player player, int malus)
    {
      player.oid.SendServerMessage($"malus appliqué : {malus}");
      player.oid.GetLocalVariable<int>("_ARENA_MALUS_APPLIED").Value = 1;
      player.menu.Close();
    }

    private static Dictionary<int, string> arenaMalusDictionary = new Dictionary<int, string>()
    {
      { 1, "Soins magiques interdits" },
      { 2, "Invocations interdites" },
      { 3, "Magie offensive interdite" },
      { 4, "Buffs interdits" },
      { 5, "Magie interdite" },
      { 6, "Accessoires interdits" },
      { 7, "Armure interdite" },
      { 8, "Armes interdites" },
      { 9, "Utilisation d'objets interdite" },
      { 10, "Ralentissement" },
      { 11, "Mini" },
      { 12, "Poison" },
      { 13, "Crapaud" },
      { 14, "Temps x5" },
      { 15, "1/2 HP" },
      { 16, "Echec des sorts" },
      { 17, "1/2 HP + Echec des sorts" },
      { 18, "Dissipation" },
      { 19, "Chance" },
      { 20, "Soins" },
    };

    private static void OnTargetSelected(ModuleEvents.OnPlayerTarget selection)
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
