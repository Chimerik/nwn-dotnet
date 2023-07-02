using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static int GetCrackedArmorModifiedDuration(NwGameObject targetObject, int duration)
    {
      if (targetObject is not NwCreature targetCreature)
        return 0;

      bool applyCrackedArmor = true;

      foreach (var eff in targetCreature.ActiveEffects)
      {
        if (eff.Tag == "CUSTOM_CONDITION_CRACKEDARMOR")
        {
          if (eff.DurationRemaining > duration)
            applyCrackedArmor = false;
          else
            targetCreature.RemoveEffect(eff);
        }
      }

      return applyCrackedArmor ? duration : 0;
    }
  }
}
