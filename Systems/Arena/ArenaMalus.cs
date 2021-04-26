using System;
using NWN.API;

namespace NWN.Systems.Arena
{
  public class ArenaMalus
  {
    public Action<PlayerSystem.Player> applyMalus { get; }
    public string name { get; }
    public double basePoints { get; }

    public ArenaMalus(string name, double basePoints, Action<PlayerSystem.Player> applyMalus)
    {
      this.name = name;
      this.applyMalus = applyMalus;
      this.basePoints = basePoints;
    }
  }
}
