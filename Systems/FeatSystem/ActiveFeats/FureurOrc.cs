using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FureurOrc(NwCreature caster)
    {
      if (caster.CurrentAction == Action.AttackObject && ItemUtils.IsMeleeWeapon(caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem))
      {
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FureurOrcBonusDamageVariable).Value = 1;
        caster.DecrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.FureurOrc));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Fureur Orc", StringUtils.gold, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Il vous faut être en train d'attaquer avec une arme de mêlée", ColorConstants.Red);
    }
  }
}
