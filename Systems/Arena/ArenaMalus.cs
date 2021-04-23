using System;
using NWN.API;

namespace NWN.Systems.Arena
{
  public class ArenaMalus
  {
    private Action<PlayerSystem.Player> applyMalus;
    public string name { get; }

    public ArenaMalus(string name, Action<PlayerSystem.Player> applyMalus)
    {
      this.name = name;
      this.applyMalus = applyMalus;
    }
  }
}
