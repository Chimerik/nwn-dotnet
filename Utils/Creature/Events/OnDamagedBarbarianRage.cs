using Anvil.API;
using Anvil.API.Events;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnDamagedBarbarianRage(CreatureEvents.OnDamaged onDamage)
    {
      onDamage.Creature.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Value = 1;
    }
  }
}
