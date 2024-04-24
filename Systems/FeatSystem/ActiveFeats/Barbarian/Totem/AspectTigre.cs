using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AspectTigre(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
      {
        if (caster.CurrentAction == Action.AttackObject && ItemUtils.IsMeleeWeapon(caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem))
        {
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.AspectTigreVariable).Value = 1;
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.TigerAspectBleedVariable).Value = 2;
          caster.OnCreatureAttack += CreatureUtils.OnAttackApplyBleed;
        }
        else
          caster.LoginPlayer?.SendServerMessage("Il vous faut être en train d'attaquer avec une arme de mêlée", ColorConstants.Red);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Vous devez être sous l'effet de Rage pour utiliser cette attaque", ColorConstants.Red);
    }
  }
}
