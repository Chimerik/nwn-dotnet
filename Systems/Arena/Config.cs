using System;
using System.Collections.Generic;
using static NWN.Systems.Arena.Utils;

namespace NWN.Systems.Arena
{
  public static class Config
  {
    public const string PVE_ARENA_AREA_RESREF = "pve_arena";
    public const string PVE_ARENA_WAYPOINT_TAG = "waypoint";
    public const string PVE_ENTRY_WAYPOINT_TAG = "arena_entry_waypoint";
    public const string PVE_ARENA_PULL_ROPE_CHAIN_TAG = "PullRopeChain";
    public const string PVE_ARENA_PULL_ROPE_CHAIN_ON_USED_SCRIPT = "arena_chain_ou";
    public const string PVE_ARENA_CREATURE_ON_DEATH_SCRIPT = "arena_ondeath";
    public const string PVE_ARENA_CHALLENGER_VARNAME = "PVE_ARENA_CHALLENGER";
    public const string PVE_ARENA_CREATURE_TAG = "pve_arena_creature";
    public const int roundMax = 8;
    public enum Difficulty
    {
      Level1,
      Level2,
      Level3,
      Level4,
      Level5,
    }

    public static RoundCreatures[] GetNormalEncounters(Difficulty difficulty) {
      switch(difficulty)
      {
        default: throw new Exception($"PvE Arena: Invalid normal encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "nw_chicken", "nw_chicken", "nw_chicken", "nw_chicken" },
            points: 1  
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_bat", "nw_bat", "nw_bat" },
            points: 1
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_rat001", "nw_rat001" },
            points: 2
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_cougar" },
            points: 5
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_jaguar" },
            points: 6
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_dog", "nw_dog" },
            points: 4
          ),
        };

        case Difficulty.Level2: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
        };
      }
    }

    public static RoundCreatures[] GetEliteEncounters(Difficulty difficulty)
    {
      switch (difficulty)
      {
        default: throw new Exception($"PvE Arena: Invalid elite encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "nw_orca" },
            points: 10
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_gnoll001" },
            points: 10
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_boar" },
            points: 10
          ),
        };

        case Difficulty.Level2: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
        };
      }
    }

    public static RoundCreatures[] GetBossEncounters(Difficulty difficulty)
    {
      switch (difficulty)
      {
        default: throw new Exception($"PvE Arena: Invalid boss encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "nw_bearbrwn" },
            points: 20
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_fire" },
            points: 20
          ),
          new RoundCreatures(
            resrefs: new string[] { "nw_spiddire" },
            points: 20
          ),
        };

        case Difficulty.Level2: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
        };
      }
    }
  }
}
