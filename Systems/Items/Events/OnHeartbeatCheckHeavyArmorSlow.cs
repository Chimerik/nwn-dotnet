using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private static void OnHeartbeatCheckHeavyArmorSlow(CreatureEvents.OnHeartbeat onHB)
    {
      if(onHB.Creature.GetAbilityScore(Ability.Strength) > 14)
      {
        onHB.Creature.RemoveEffect(slow);
        onHB.Creature.OnHeartbeat -= OnHeartbeatCheckHeavyArmorSlow;
      }
    }
  }
}
