using System.Collections.Generic;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Arena.Config;
using Anvil.API;
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
    public static void DrawNextFightPage(Player player, SpellSystem spellSystem)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>()
        {
          $"Vous remportez ce round. Points de la tentative : {player.pveArena.currentPoints.ToString().ColorString(ColorConstants.Red)} points de victoire !",
          "Que souhaitez vous faire ?"
        };

      player.menu.choices.Add((
        "Combat suivant !",
        () => DrawMalusSelectionPage(player, spellSystem)
      ));
      player.menu.choices.Add((
        "S'arrêter et conserver les points accumulés",
        () => HandleStop(player)
      ));

      player.menu.Draw();
    }
    public static void DrawMalusSelectionPage(Player player, SpellSystem spellSystem)
    {
      player.oid.LoginCreature.RemoveFeat(CustomFeats.CustomMenuEXIT);
      player.oid.LoginCreature.RemoveFeat(CustomFeats.CustomMenuUP);
      player.oid.LoginCreature.RemoveFeat(CustomFeats.CustomMenuDOWN);

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
          "Arrêtez la roulette pour définir votre handicap !",
          }; 

      uint random = (uint)NwRandom.Roll(NWN.Utils.random, 20);

      player.menu.choices.Add((
        arenaMalusDictionary[random].name,
        () => Utils.ApplyArenaMalus(player, random, spellSystem)
      ));

      foreach (string malus in player.pveArena.currentMalusList)
        player.menu.choices.Add((
        malus,
        () => Utils.ApplyArenaMalus(player, random, spellSystem)
      ));

      player.menu.Draw();
      Utils.RandomizeMalusSelection(player, spellSystem);
    }
    public static void HandleStop(Player player)
    {
      player.pveArena.totalPoints += player.pveArena.currentPoints;
      HandleRunAway(player);
    }
    private static void HandleRunAway(Player player)
    {
      player.menu.Close();
      player.oid.LoginCreature.ClearActionQueue();
      player.oid.LoginCreature.Location = NwObject.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;
    }
  }
}
