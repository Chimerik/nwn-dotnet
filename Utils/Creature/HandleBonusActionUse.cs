using Anvil.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool HandleBonusActionUse(NwCreature creature)
    {
      if (creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value < 1)
      {
        creature.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Red);
        return false;
      }

      creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value -= 1;
      HandleBonusActionCooldown(creature);

      return true;
    }
  }
}
