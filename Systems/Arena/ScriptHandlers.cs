using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  public static class ScriptHandlers
  {
    public static void HandlePullRopeChainUse(PlaceableEvents.OnUsed onUsed)
    {
      NwCreature oPC = onUsed.UsedBy;

      if (Players.TryGetValue(oPC, out Player player))
        ArenaMenu.DrawRunAwayPage(player);
    }
    public static void HandleFight(Player player)
    {
      NwArea oArena = player.oid.Area;
      NwWaypoint oWaypoint = oArena.FindObjectsOfTypeInArea<NwWaypoint>().Where(w => w.Tag == Config.PVE_ARENA_WAYPOINT_TAG).FirstOrDefault();
      var roundCreatures = Utils.GetCreaturesForRound(player.pveArena.currentRound, player.pveArena.currentDifficulty);

      foreach (var creatureResref in roundCreatures.resrefs)
      {
        NwCreature creature = NwCreature.Create(creatureResref, oWaypoint.Location, true, Config.PVE_ARENA_CREATURE_TAG);
        creature.ChangeToStandardFaction(StandardFaction.Hostile);
      }

      Task waitRoundCreaturesDead = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => player.oid.Area.FindObjectsOfTypeInArea<NwCreature>().Any(c => c.Tag == Config.PVE_ARENA_CREATURE_TAG) == false);
        
        Log.Info($"{player.oid.Name} just finished round {player.pveArena.currentRound} - Difficulty {player.pveArena.currentDifficulty}");
        player.pveArena.currentPoints += (uint)player.pveArena.currentDifficulty * player.pveArena.currentRound * player.pveArena.currentMalus;
        // TODO : ajouter les points du malus
        player.pveArena.currentRound += 1;
        
        if(player.pveArena.currentRound >= Config.roundMax)
        {
          ArenaMenu.HandleStop(player);
          player.oid.SendServerMessage($"Félicitations, vous avez terminé vos combats et remporté {player.pveArena.currentPoints.ToString().ColorString(Color.WHITE)} sur cette tentative pour un total de {player.pveArena.totalPoints.ToString().ColorString(Color.WHITE)}", Color.ROSE);
        }
        else
        {
          Effect paralyze = Effect.CutsceneParalyze();
          paralyze.SubType = EffectSubType.Supernatural;
          paralyze.Tag = "_ARENA_CUTSCENE_PARALYZE_EFFECT";
          player.oid.ApplyEffect(EffectDuration.Permanent, paralyze);

          //Utils.RandomizeMalusSelection(player);

          ArenaMenu.DrawNextFightPage(player);
        }
      });
    }
  }
}
