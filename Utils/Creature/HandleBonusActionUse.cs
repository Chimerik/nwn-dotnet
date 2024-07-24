using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool HandleBonusActionUse(NwCreature creature)
    {
      if (creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value > 0 || creature.IsDMPossessed || creature.IsDMAvatar)
      {
        if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.LenteurEffectTag))
        {
          creature.LoginPlayer?.SendServerMessage("Sous l'effet de lenteur vous ne pouvez pas faire usage d'action bonus", ColorConstants.Red);
          return false;
        }

        creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value -= 1;
        HandleBonusActionCooldown(creature);
        return true;
      }

      creature.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Red);
      return false;
    }
  }
}
