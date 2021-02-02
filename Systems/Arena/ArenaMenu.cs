using System.Collections.Generic;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Arena.Config;

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

      var oArena = NWScript.GetArea(player.oid);
      // TODO - Ajouter un malus aléatoire au joueur
      var oWaypoint = AreaUtils.GetObjectInAreaByTag(oArena, PVE_ARENA_WAYPOINT_TAG);
      var roundCreatures = Utils.GetCreaturesForRound(player.pveArena.currentRound, player.pveArena.currentDifficulty);
      player.pveArena.potentialPoints = roundCreatures.points;

      foreach (var creatureResref in roundCreatures.resrefs)
      {
        var oCreature = NWScript.CreateObject(
          NWScript.OBJECT_TYPE_CREATURE,
          creatureResref,
          NWScript.GetLocation(oWaypoint),
          1,
          PVE_ARENA_CREATURE_TAG
        );
        NWScript.SetLocalInt(oCreature, PVE_ARENA_CHALLENGER_VARNAME, (int)player.oid);
        NWScript.SetEventScript(oCreature, NWScript.EVENT_SCRIPT_CREATURE_ON_DEATH, PVE_ARENA_CREATURE_ON_DEATH_SCRIPT);
        NWScript.ChangeToStandardFaction(oCreature, NWScript.STANDARD_FACTION_HOSTILE);
      }
    }

    private static void HandleStop(Player player)
    {
      player.menu.Close();
      Utils.StopCurrentRun(player);
      var oArea = NWScript.GetArea(player.oid);
      if (AreaSystem.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(oArea), out Area area))
      {
        area.DeferDestroy();
      }
      var oWaypoint = NWScript.GetObjectByTag(PVE_ENTRY_WAYPOINT_TAG);
      var location = NWScript.GetLocation(oWaypoint);
      NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions());
      NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(location));

      player.OnDeath -= Utils.HandlePlayerDied;
    }
  }
}
