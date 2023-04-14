using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.Arena.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  public static class WelcomeMenu
  {
    public static void DrawMainPage(Player player, SpellSystem spellSystem)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Bienvenue dans l'arène de la Couronne de Cuivre !",
        "Que puis-je faire pour vous aujourd'hui ?"
      };

      player.menu.choices.Add((
        "M'inscrire pour participer aux prochains combats",
        () => DrawSubcribePage(player, spellSystem)
      ));
      player.menu.choices.Add((
        "Dépenser mes points de victoires pour acheter des récompenses",
        () => OpenArenaRewardShop(player)
      ));
      if (player.oid.PlayerName == "Chim")
      {
        player.menu.choices.Add((
            "Modifier les récompenses",
            () => ModifyArenaRewardShop(player)
          ));
      }
      player.menu.choices.Add((
          "Afficher la liste des combats en cours",
          () => DrawCurrentRunList(player, spellSystem)
        ));
      player.menu.choices.Add((
          "Quitter",
          () => player.menu.Close()
        ));

      player.menu.Draw();
    }

    private static void DrawSubcribePage(Player player, SpellSystem spellSystem)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Veuillez choisir votre niveau de difficulté");
      player.menu.choices.Add((
         "1. C'est ma première fois, ne tapez pas trop fort s'il vous plaît !",
         () => DrawConfirmPage(player, Difficulty.Level1, spellSystem)
      ));

      player.menu.choices.Add((
         "Retour",
         () => DrawMainPage(player, spellSystem)
      ));
      player.menu.choices.Add((
          "Quitter",
          () => player.menu.Close()
        ));

      player.menu.Draw();
    }

    private static void DrawConfirmPage(Player player, Difficulty difficulty, SpellSystem spellSystem)
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
          HandleConfirm(player, spellSystem);
        }
      ));
      player.menu.choices.Add((
        "Non, peut-être une autre fois.",
        () => player.menu.Close()
      ));

      player.menu.Draw();
    }

    private static async void HandleConfirm(Player player, SpellSystem spellSystem)
    {
      player.menu.Close();
      player.pveArena.dateArenaEntered = DateTime.Now;
      
      NwArea oArena = NwArea.Create(PVE_ARENA_AREA_RESREF);
      oArena.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(w => w.Tag == PVE_ARENA_PULL_ROPE_CHAIN_TAG).OnLeftClick += spellSystem.HandlePullRopeChainUse;
      oArena.OnExit += spellSystem.OnExitArena;

      player.oid.LoginCreature.Location = oArena.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(w => w.Tag == PVE_ARENA_WAYPOINT_TAG).Location;
      
      player.oid.OnPlayerDeath -= HandlePlayerDeath;
      player.oid.OnPlayerDeath += Utils.HandleArenaDeath;

      await NwTask.WaitUntil(() => player.oid.ControlledCreature.Area != null);
      await NwTask.Delay(TimeSpan.FromSeconds(5));
      ScriptHandlers.HandleFight(player, spellSystem);
    }

    private static void OpenArenaRewardShop(Player player)
    {
      player.menu.Close();

      NwStore shop = player.oid.ControlledCreature.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == "arena_reward_shop");

      if (shop == null)
      {
        var query = SqLiteUtils.SelectQuery("arenaRewardShop",
        new List<string>() { { "shop" } },
        new List<string[]>() );

        if (query == null)
        {
          player.oid.SendServerMessage("La boutique de récompenses n'a pas encore été initialisée. Le staff a été prévenu de cette erreur", ColorConstants.Red);
          NWN.Utils.LogMessageToDMs("La boutique de récompense de l'arène PvE n'est pas initialisée.");
          return;
        }
        else
        {
          shop = SqLiteUtils.StoreSerializationFormatProtection(query.FirstOrDefault()[0], player.oid.ControlledCreature.Location);

          foreach (NwItem item in shop.Items)
            item.BaseGoldValue = (uint)(item.GetObjectVariable<LocalVariableInt>("_SET_SELL_PRICE").Value);
        }
      }

      shop.OnOpen -= StoreSystem.OnOpenArenaRewardStore;
      shop.OnOpen += StoreSystem.OnOpenArenaRewardStore;
      shop.Open(player.oid);
    }

    private static void DrawCurrentRunList(Player player, SpellSystem spellSystem)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("");
      player.menu.titleLines = new List<string>()
        {
          "Voici la liste des combats auquels vous pouvez assister.",
          "Sélectionnez le spectacle auquel vous souhaitez assister."
        };

      foreach (NwPlayer oPC in NwModule.Instance.Players)
      {
        if (Players.TryGetValue(oPC.LoginCreature, out Player fighter) && fighter.pveArena.currentRound > 0)
        {
          Player targetToObserve = fighter;
          player.menu.choices.Add((
          $"{oPC.LoginCreature.Name} - Difficulté {fighter.pveArena.currentDifficulty} - Round {fighter.pveArena.currentRound} - Points {fighter.pveArena.currentPoints}",
          () => SpectateArena(player, targetToObserve, spellSystem)
          ));
        }
      }

      player.menu.choices.Add((
          "Retour",
          () => DrawMainPage(player, spellSystem)
        ));
      player.menu.choices.Add((
          "Quitter",
          () => player.menu.Close()
        ));

      player.menu.Draw();
    }
    private static void ModifyArenaRewardShop(Player player)
    {
      player.menu.Close();

      NwStore shop = player.oid.ControlledCreature.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == "arena_reward_shop");

      if (shop == null)
      {
        var query = SqLiteUtils.SelectQuery("arenaRewardShop",
          new List<string>() { { "shop" } },
          new List<string[]>() { new string[] { "id", "1" } });

        if (query == null)
        {
          shop = NwStore.Create("generic_shop_res", player.oid.ControlledCreature.Location);
        }
        else
        {
          shop = SqLiteUtils.StoreSerializationFormatProtection(query.FirstOrDefault()[0], player.oid.ControlledCreature.Location);

          foreach (NwItem item in shop.Items)
            item.BaseGoldValue = (uint)(item.GetObjectVariable<LocalVariableInt>("_SET_SELL_PRICE").Value);
        }
      }

      shop.OnOpen -= StoreSystem.OnOpenModifyArenaRewardStore;
      shop.OnOpen += StoreSystem.OnOpenModifyArenaRewardStore;
      shop.Open(player.oid);
    }
    private static void SpectateArena(Player player, Player playerToSpectate, SpellSystem spellSystem)
    {
      player.menu.Close();

      if(playerToSpectate.pveArena.currentRound == 0)
      {
        player.oid.SendServerMessage($"{playerToSpectate} n'est plus en combat. Veuillez choisir un autre spectacle.");
        DrawCurrentRunList(player, spellSystem);
        return;
      }

      player.oid.LoginCreature.Location = NwObject.FindObjectsWithTag<NwWaypoint>("_SPECTATOR_WAYPOINT").FirstOrDefault().Location;

      //player.oid.LoginCreature.OnSpellCast -= spellSystem.CheckIsDivinationBeforeSpellCast;
      player.oid.LoginCreature.OnSpellCast -= Utils.NoMagicMalus;
      player.oid.LoginCreature.OnSpellCast += Utils.NoMagicMalus;
    }
  }
}
