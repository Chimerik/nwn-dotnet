using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool HandleBonusActionUse(NwCreature creature)
    {
      if (creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value > 0 || creature.IsDMPossessed || creature.IsDMAvatar)
      {
        creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value -= 1;
        HandleBonusActionCooldown(creature);
        return true;
      }

      creature.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Red);
      return false;
    }
  }
}
