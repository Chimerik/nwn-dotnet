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
        () => ScriptHandlers.HandleFight(player)
      ));
      player.menu.choices.Add((
        "S'arrêter et conserver les points accumulés",
        () => HandleStop(player)
      ));


      player.menu.Draw();
    }

    public static void HandleStop(Player player)
    {
      player.menu.Close();
      Utils.StopCurrentRun(player);

      //AreaSystem.AreaDestroyer(oArea);
      
      player.oid.ClearActionQueue();
      player.oid.Location = NwModule.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault()?.Location;
      player.OnDeath -= Utils.HandlePlayerDied;
    }
    private static void HandleRunAway(Player player)
    {
      player.menu.Close();
      Utils.CancelCurrentRun(player);

      //AreaSystem.AreaDestroyer(oArea);

      player.oid.ClearActionQueue();
      player.oid.Location = NwModule.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault()?.Location;
      player.OnDeath -= Utils.HandlePlayerDied;
    }
  }
}
