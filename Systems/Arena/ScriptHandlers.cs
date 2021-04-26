using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  public static class ScriptHandlers
  {
    public static void HandlePullRopeChainUse(PlaceableEvents.OnUsed onUsed)
    {
      NwCreature oPC = onUsed.UsedBy;

      if (Players.TryGetValue(oPC, out Player player))
      {
        if (player.oid.Area.FindObjectsOfTypeInArea<NwCreature>().Any(c => c.Tag == Config.PVE_ARENA_CREATURE_TAG))
          ArenaMenu.DrawRunAwayPage(player);
        else
          ArenaMenu.DrawNextFightPage(player);
      }
    }
    public static async void HandleFight(Player player)
    {
      NwArea oArena = player.oid.Area;
      NwWaypoint oWaypoint = oArena.FindObjectsOfTypeInArea<NwWaypoint>().Where(w => w.Tag == Config.PVE_ARENA_MONSTER_WAYPOINT_TAG).FirstOrDefault();
      var roundCreatures = Utils.GetCreaturesForRound(player.pveArena.currentRound, player.pveArena.currentDifficulty);

      foreach (var creatureResref in roundCreatures.resrefs)
      {
        NwCreature creature = NwCreature.Create(creatureResref, oWaypoint.Location, true, Config.PVE_ARENA_CREATURE_TAG);
        creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster3));
        creature.ChangeToStandardFaction(StandardFaction.Hostile);
      }

      NwArea area = player.oid.Area;

      CancellationTokenSource tokenSource = new CancellationTokenSource();
      Task waitPlayerLeavesArena = NwTask.WaitUntil(() => player.oid.Location.Area == null, tokenSource.Token);
      Task waitRoundCreaturesDead = NwTask.WaitUntil(() => !area.FindObjectsOfTypeInArea<NwCreature>().Any(c => c.Tag == Config.PVE_ARENA_CREATURE_TAG), tokenSource.Token);

      await NwTask.WhenAny(waitRoundCreaturesDead, waitPlayerLeavesArena);
      tokenSource.Cancel();

      if (waitPlayerLeavesArena.IsCompletedSuccessfully)
        return;

      Log.Info($"{player.oid.Name} just finished round {player.pveArena.currentRound} - Difficulty {player.pveArena.currentDifficulty}");
      player.pveArena.currentPoints += GetRoundPoints(player);
      player.pveArena.currentRound += 1;

      if (player.pveArena.currentRound > Config.roundMax)
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

        ArenaMenu.DrawNextFightPage(player);
      }
    }
    private static uint GetRoundPoints(Player player)
    {
      if (player.pveArena.currentRound == 8)
      {
        return (uint)(Math.Pow(Config.arenaMalusDictionary[player.pveArena.currentMalus].basePoints, player.pveArena.currentRound) * (int)player.pveArena.currentDifficulty);
      }
      else
      {
        double result = (Math.Pow(Config.arenaMalusDictionary[player.pveArena.currentMalus].basePoints, player.pveArena.currentRound) - Math.Pow(Config.arenaMalusDictionary[player.pveArena.currentMalus].basePoints, player.pveArena.currentRound - 1)) * (int)player.pveArena.currentDifficulty;
        if (result < 0)
          result = 1;

        return (uint)result;
      }
    }
  }
}
