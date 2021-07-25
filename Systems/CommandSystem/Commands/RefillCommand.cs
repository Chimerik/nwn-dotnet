using NWN.Core;

namespace NWN.Systems
{
  class Refill
  {
    public Refill()
    {
      _ = ModuleSystem.SpawnCollectableResources(0.0f);
    }
  }
}
