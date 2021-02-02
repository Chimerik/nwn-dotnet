using System;
using System.Collections.Generic;
using NWN.Core;
using static NWN.Systems.Arena.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  public static class WelcomeMenu
  {
    public static void DrawMainPage(Player player)
    {
      player.menu.Clear();

      if (player.pveArena.currentRound > 1)
      {
        player.menu.titleLines = new List<string>()
        {
          "Bienvenue dans l'arène de Similisse !",
          "Il semblerait que vous ayez déjà un combat en court.",
          "Que souhaitez-vous faire ?"
        };

        player.menu.choices.Add((
          "Continuer les combats",
          () => HandleConfirm(player)
        ));
        player.menu.choices.Add((
          $"Annuler et conserver les points acquis ({player.pveArena.currentPoints})",
          () => Utils.StopCurrentRun(player)
        ));
      } else
      {
        player.menu.titleLines = new List<string>() {
        "Bienvenue dans l'arene de Similisse !",
        "Que puis-je faire pour vous aujourd'hui ?"
      };
        player.menu.choices.Add((
          "M'inscrire pour participer aux prochains combats",
          () => DrawSubcribePage(player)
        ));
        player.menu.choices.Add((
          "Depenser mes points de victoire pour acheter des récompenses",
          () => DrawRewardPage(player)
        ));
        player.menu.choices.Add((
          "Voir la liste des meilleurs combattants",
          () => DrawHighscoresPage(player)
        ));
      }

      player.menu.Draw();
    }

    private static void DrawSubcribePage(PlayerSystem.Player player)
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
      var oArena = NWScript.CreateArea(PVE_ARENA_AREA_RESREF);
      var oWaypoint = AreaUtils.GetObjectInAreaByTag(oArena, PVE_ARENA_WAYPOINT_TAG);
      var oPullRopeChain = AreaUtils.GetObjectInAreaByTag(oArena, PVE_ARENA_PULL_ROPE_CHAIN_TAG);
      NWScript.SetEventScript(
        oPullRopeChain,
        NWScript.EVENT_SCRIPT_PLACEABLE_ON_USED,
        PVE_ARENA_PULL_ROPE_CHAIN_ON_USED_SCRIPT
      );
      var location = NWScript.GetLocation(oWaypoint);
      NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions());
      NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(location));
      player.OnDeath += Utils.HandlePlayerDied;
    }

    private static void DrawRewardPage(PlayerSystem.Player player)
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
  }
}
