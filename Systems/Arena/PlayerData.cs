﻿
using System;
using System.Collections.Generic;

namespace NWN.Systems.Arena
{
  public class PlayerData
  {
    public uint currentRound { get; set; }
    public Config.Difficulty currentDifficulty { get; set; }
    public uint currentPoints { get; set; }
    public uint totalPoints { get; set; }
    public uint potentialPoints { get; set; }
    public uint currentMalus { get; set; }
    public DateTime dateArenaEntered { get; set; }
    public List<string> currentMalusList { get; set; }
    public PlayerData(
      uint currentRound = 0, Config.Difficulty currentDifficulty = Config.Difficulty.Level1,
      uint currentPoints = 0, uint totalPoints = 0, uint potentialPoints = 0, uint currentMalus = 0
    )
    {
      this.currentRound = currentRound;
      this.currentDifficulty = currentDifficulty;
      this.currentPoints = currentPoints;
      this.totalPoints = totalPoints;
      this.potentialPoints = potentialPoints;
      this.currentMalus = currentMalus;
      this.dateArenaEntered = DateTime.Now;
      currentMalusList = new List<string>();
    }
  }
}
