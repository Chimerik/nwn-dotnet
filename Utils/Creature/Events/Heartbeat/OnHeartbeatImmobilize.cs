using Anvil.API;
using Anvil.API.Events;
namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnHeartbeatImmobilize(CreatureEvents.OnHeartbeat onHB)
    {
      onHB.Creature.MovementRate = MovementRate.Immobile;
      onHB.Creature.MovementRateFactor = 0;
    }
  }
}
