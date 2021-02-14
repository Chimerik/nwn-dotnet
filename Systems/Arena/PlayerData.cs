
namespace NWN.Systems.Arena
{
  public struct PlayerData
  {
    public uint currentRound { get; set; }
    public Config.Difficulty currentDifficulty { get; set; }
    public uint currentPoints { get; set; }
    public uint totalPoints { get; set; }
    public uint potentialPoints { get; set; }

    public PlayerData(
      uint currentRound = 1, Config.Difficulty currentDifficulty = Config.Difficulty.Level1,
      uint currentPoints = 0, uint totalPoints = 0, uint potentialPoints = 0
    )
    {
      this.currentRound = currentRound;
      this.currentDifficulty = currentDifficulty;
      this.currentPoints = currentPoints;
      this.totalPoints = totalPoints;
      this.potentialPoints = potentialPoints;
    }
  }
}
