using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappeFrenetique(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value > 0)
      {
        if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
        {
          if (caster.CurrentAction == Action.AttackObject && ItemUtils.IsMeleeWeapon(caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem))
          {
            caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeFrenetiqueVariable).Value = 1;
            caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeFrenetiqueMalusVariable).Value += 1;
            caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

            CreatureUtils.HandleBonusActionCooldown(caster);
          }
          else
            caster.LoginPlayer?.SendServerMessage("Il vous faut être en train d'attaquer avec une arme de mêlée", ColorConstants.Red);
        }
        else
          caster.LoginPlayer?.SendServerMessage("Vous devez être sous l'effet de Rage pour utiliser cette attaque", ColorConstants.Red);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Red);
    }
  }
}
