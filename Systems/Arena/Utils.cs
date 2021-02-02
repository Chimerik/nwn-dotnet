using System;
using NWN.Core;
using static NWN.Systems.Arena.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  public static class Utils
  {
    public static int GetGoldEntryCost(Difficulty difficulty)
    {
      switch(difficulty)
      {
        default: return 0;

        case Difficulty.Level1: return 50;
        case Difficulty.Level2: return 500;
        case Difficulty.Level3: return 1000;
        case Difficulty.Level4: return 2000;
        case Difficulty.Level5: return 5000;
      }
    }

    public static void StopCurrentRun(Player player)
    {
      player.pveArena.totalPoints += player.pveArena.currentPoints;
      player.pveArena.currentPoints = 0;
      player.pveArena.currentRound = 1;
    }

    public static bool GetIsRoundInProgress(Player player)
    {
      var creature = NWScript.GetNearestObjectByTag(PVE_ARENA_CREATURE_TAG, player.oid);

      return NWScript.GetIsObjectValid(creature) == 1;
    }

    public static Action<Player> CheckRoundEnded = TimingUtils.Debounce((Player player) =>
    {
      if (!GetIsRoundInProgress(player))
      {
        player.pveArena.currentPoints += player.pveArena.potentialPoints;
        player.pveArena.currentRound += 1;
      }
    }, 0.5f);

    public static void HandlePlayerDied(object sender, Player.DeathEventArgs e)
    {
      e.player.OnDeath -= HandlePlayerDied;
      e.player.pveArena.currentPoints = 0;
      e.player.pveArena.currentRound = 1;

      var oArea = NWScript.GetArea(e.player.oid);

      if (AreaSystem.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(oArea), out Area area))
      {
        area.DeferDestroy();
      }
    }

    public struct RoundCreatures
    {
      public string[] resrefs;
      public uint points;

      public RoundCreatures(string[] resrefs, uint points)
      {
        this.resrefs = resrefs;
        this.points = points;
      }
    }

    public static RoundCreatures GetCreaturesForRound(uint round, Difficulty difficulty)
    {
      switch (round)
      {
        default: return GetRandomNormalEncounter(difficulty);

        case 4: return GetRandomEliteEncounter(difficulty);
        case 8: return GetRandomBossEncounter(difficulty);
      }
    }

    public static RoundCreatures GetRandomNormalEncounter(Difficulty difficulty)
    {
      var encounters = GetNormalEncounters(difficulty);

      return encounters[NWN.Utils.random.Next(0, encounters.Length - 1)];
    }

    public static RoundCreatures GetRandomEliteEncounter(Difficulty difficulty)
    {
      var encounters = GetEliteEncounters(difficulty);

      return encounters[NWN.Utils.random.Next(0, encounters.Length - 1)];
    }

    public static RoundCreatures GetRandomBossEncounter(Difficulty difficulty)
    {
      var encounters = GetBossEncounters(difficulty);

      return encounters[NWN.Utils.random.Next(0, encounters.Length - 1)];
    }
  }
}
