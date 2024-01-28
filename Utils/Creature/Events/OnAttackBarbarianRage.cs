using Anvil.API;
using Anvil.API.Events;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackBarbarianRage(OnCreatureAttack onAttack)
    {
      onAttack.Attacker.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Value = 1;
    }
  }
}
