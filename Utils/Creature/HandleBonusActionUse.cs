using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool HandleBonusActionUse(NwCreature creature)
    {
      if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.LenteurEffectTag))
      {
        creature.LoginPlayer?.SendServerMessage("Sous l'effet de lenteur vous ne pouvez pas faire usage d'action bonus", ColorConstants.Red);

        if (creature.GetObjectVariable<LocalVariableObject<NwGameObject>>(CurrentAttackTarget).HasValue)
          _ = creature.ActionAttackTarget(creature.GetObjectVariable<LocalVariableObject<NwGameObject>>(CurrentAttackTarget).Value);

        return false;
      }

      Effect bonusActionEffect = creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.BonusActionEffectTag);

      if (bonusActionEffect is not null)
      {
        creature.RemoveEffect(bonusActionEffect);
        HandleBonusActionCooldown(creature);
        return true;
      }

      creature.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Red);

      if (creature.GetObjectVariable<LocalVariableObject<NwGameObject>>(CurrentAttackTarget).HasValue)
        _ = creature.ActionAttackTarget(creature.GetObjectVariable<LocalVariableObject<NwGameObject>>(CurrentAttackTarget).Value);

      return false;
    }
  }
}
