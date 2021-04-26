using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.System;
using static NWN.Systems.Arena.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  public static class WelcomeMenu
  {
    public static void DrawMainPage(Player player)
    {
      player.menu.Clear();

        player.menu.titleLines = new List<string>() {
        "Bienvenue dans l'arène de la Couronne de Cuivre !",
        "Que puis-je faire pour vous aujourd'hui ?"
      };
        player.menu.choices.Add((
          "M'inscrire pour participer aux prochains combats",
          () => DrawSubcribePage(player)
        ));
        player.menu.choices.Add((
          "Depenser mes points de victoires pour acheter des récompenses",
          () => DrawRewardPage(player)
        ));
      if (player.oid.PlayerName == "Chim")
      {
        player.menu.choices.Add((
            "Modifier les récompenses",
            () => OpenArenaRewardShop(player)
          ));
      }
      player.menu.choices.Add((
          "Voir la liste des meilleurs combattants",
          () => DrawHighscoresPage(player)
        ));
      player.menu.choices.Add((
          "Quitter",
          () => player.menu.Close()
        ));

      player.menu.Draw();
    }

    private static void DrawSubcribePage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Veuillez choisir votre niveau de difficulté");
      player.menu.choices.Add((
         "1. C'est ma première fois, ne tapez pas trop fort s'il vous plaît !",
         () => DrawConfirmPage(player, Difficulty.Level1)
      ));

      player.menu.choices.Add((
         "Retour",
         () => DrawMainPage(player)
      ));
      player.menu.choices.Add((
          "Quitter",
          () => player.menu.Close()
        ));

      player.menu.Draw();
    }

    private static void DrawConfirmPage(Player player, Difficulty difficulty)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Très bien, il vous en coûtera {Utils.GetGoldEntryCost(difficulty)} PO.",
        "Etes-vous prêt à rentrer dans l'arène ?"
      };
      player.menu.choices.Add((
        "Absolument, je suis prêt à en découdre !",
        () =>
        {
          player.PayOrBorrowGold(Utils.GetGoldEntryCost(difficulty));
          player.pveArena.currentRound = 1;
          player.pveArena.currentMalus = 19;
          player.pveArena.currentDifficulty = difficulty;
          HandleConfirm(player);
        }
      ));
      player.menu.choices.Add((
        "Non, peut-être une autre fois.",
        () => player.menu.Close()
      ));

      player.menu.Draw();
    }

    private static void HandleConfirm(Player player)
    {
      player.menu.Close();
      player.pveArena.dateArenaEntered = DateTime.Now;

      NwArea oArena = NwArea.Create(PVE_ARENA_AREA_RESREF);
      oArena.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(w => w.Tag == PVE_ARENA_PULL_ROPE_CHAIN_TAG).OnUsed += ScriptHandlers.HandlePullRopeChainUse;
      oArena.OnExit += Utils.OnExitArena;

      player.oid.ClearActionQueue();
      player.oid.JumpToObject(oArena.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(w => w.Tag == PVE_ARENA_WAYPOINT_TAG));
      
      player.oid.OnPlayerDeath -= HandlePlayerDeath;
      player.oid.OnPlayerDeath += Utils.HandlePlayerDied;

      Task waitAreaLoaded = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.oid.Area != null);
        await NwTask.Delay(TimeSpan.FromSeconds(5));
        ScriptHandlers.HandleFight(player);
      });
    }

    private static void DrawRewardPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string>() {
        $"Vous avez actuellement {player.pveArena.totalPoints} points de victoire.",
        "Voici la liste des récompenses disponibles :"
      };
      player.menu.choices.Add((
        "Retour",
        () => DrawMainPage(player)
      ));
      player.menu.Draw();
    }

    private static void DrawHighscoresPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Voici la liste de nos meilleurs champions :");
      player.menu.choices.Add((
        "Retour",
        () => DrawMainPage(player)
      ));
      player.menu.Draw();
    }
    private static void OpenArenaRewardShop(Player player)
    {
      NwStore shop = player.oid.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == "arena_reward_shop");

      if (shop == null)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"SELECT shop facing FROM arenaRewardShop");
        NWScript.SqlStep(query);

        shop = NwStore.Deserialize(NWScript.SqlGetString(query, 0).ToByteArray());

        foreach (NwItem item in shop.Items)
        {
          ItemPlugin.SetBaseGoldPieceValue(item, item.GetLocalVariable<int>("_SET_SELL_PRICE").Value);
        }
      }

      shop.OnOpen += StoreSystem.OnOpenModifyArenaRewardStore;
      shop.Open(player.oid);
    }
  }
}
