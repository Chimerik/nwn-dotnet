using System.Collections.Generic;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Arena.Config;
using NWN.API;
using System.Linq;

namespace NWN.Systems.Arena
{
  public static class ArenaMenu
  {
    public static void DrawMainPage(Player player)
    {
      player.menu.Clear();

      if (player.pveArena.currentRound > roundMax)
      {
        player.menu.titleLines = new List<string>()
        {
          "Félicitations, vous avez terminé vos combats !",
          $"Vous avez gagné {player.pveArena.currentPoints} points de victoire."
        };

        player.menu.choices.Add((
          "Valider et quitter",
          () => HandleStop(player)
        ));
      } else
      {
        player.menu.titleLines = new List<string>()
        {
          $"Vous avez accumulez {player.pveArena.currentPoints} points de victoire !",
          "Que souhaitez vous faire ?"
        };

        player.menu.choices.Add((
          "Se préparer à combattre !",
          () => HandleFight(player)
        ));
        player.menu.choices.Add((
          "S'arrêter et conserver les points accumulés",
          () => HandleStop(player)
        ));
      }

      player.menu.Draw();
    }

    private static void HandleFight(Player player)
    {
      player.menu.Close();

      NwArea oArena = player.oid.Area;
      // TODO - Ajouter un malus aléatoire au joueur
      NwWaypoint oWaypoint = oArena.FindObjectsOfTypeInArea<NwWaypoint>().Where(w => w.Tag == PVE_ARENA_WAYPOINT_TAG).FirstOrDefault();
      var roundCreatures = Utils.GetCreaturesForRound(player.pveArena.currentRound, player.pveArena.currentDifficulty);
      player.pveArena.potentialPoints = roundCreatures.points;

      foreach (var creatureResref in roundCreatures.resrefs)
      {
        NwCreature creature = NwCreature.Create(creatureResref, oWaypoint.Location, true, PVE_ARENA_CREATURE_TAG);
        NWScript.SetLocalInt(creature, PVE_ARENA_CHALLENGER_VARNAME, player.characterId);
        creature.OnDeath += ScriptHandlers.HandleCreatureOnDeath;
        creature.ChangeToStandardFaction(API.Constants.StandardFaction.Hostile);
      }
    }

    private static void HandleStop(Player player)
    {
      player.menu.Close();
      Utils.StopCurrentRun(player);
      NwArea oArea = player.oid.Area;
      AreaSystem.AreaDestroyer(oArea);
      player.oid.ClearActionQueue();

      //NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NwModule.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault()?.Location));
      player.oid.Location = NwModule.FindObjectsWithTag<NwWaypoint>(PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault()?.Location;

      player.OnDeath -= Utils.HandlePlayerDied;
    }
  }
}
