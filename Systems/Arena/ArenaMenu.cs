using System.Collections.Generic;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Arena.Config;
using NWN.API;
using System.Linq;

namespace NWN.Systems.Arena
{
  public static class ArenaMenu
  {
    public static void DrawRunAwayPage(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>()
        {
          "Souhaitez-vous fuir le combat ?",
          $"Attention, cela signifie que vous perdez les {player.pveArena.currentPoints} accumulés pour cette tentative !"
        };

      player.menu.choices.Add((
          "Fuir le combat",
          () => HandleRunAway(player)
        ));
      player.menu.choices.Add((
          "QUE TREPASSE SI JE FAIBLIS !",
          () => player.menu.Close()
        ));

      player.menu.Draw();
    }
    public static void DrawNextFightPage(Player player)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>()
        {
          $"Vous remportez ce round. Points de la tentative : {player.pveArena.currentPoints.ToString().ColorString(Color.RED)} points de victoire !",
          "Que souhaitez vous faire ?"
        };

      player.menu.choices.Add((
        "Combat suivant !",
        () => DrawMalusSelectionPage(player)
      ));
      player.menu.choices.Add((
        "S'arrêter et conserver les points accumulés",
        () => HandleStop(player)
      ));


      player.menu.Draw();
    }
    public static void DrawMalusSelectionPage(Player player)
    {
      player.oid.RemoveFeat(CustomFeats.CustomMenuEXIT);

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
          "Sélectionnez votre malus !",
          };

      uint random = (uint)NwRandom.Roll(NWN.Utils.random, 20);

      player.menu.choices.Add((
        arenaMalusDictionary[random].name,
        () => Utils.ApplyArenaMalus(player, random)
      ));

      player.menu.Draw();
      Utils.RandomizeMalusSelection(player);
    }
    public static void HandleStop(Player player)
    {
      player.menu.Close();
      player.pveArena.totalPoints += player.pveArena.currentPoints;

      player.oid.ClearActionQueue();
      player.oid.Location = NwModule.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;
    }
    private static void HandleRunAway(Player player)
    {
      player.menu.Close();
      player.oid.ClearActionQueue();
      player.oid.Location = NwModule.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault()?.Location;
    }
  }
}
