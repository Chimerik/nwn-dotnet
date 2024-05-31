using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamagedBarbarianRage(CreatureEvents.OnDamaged onDamaged)
    {
      onDamaged.Creature.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Value = 1;
    }
  }
}
