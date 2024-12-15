using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyCoupeJarret()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseCoupeJarret))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipCoupeJarret;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipCoupeJarret;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipCoupeJarret;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipCoupeJarret;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortbow, BaseItemType.Longbow, BaseItemType.ThrowingAxe, BaseItemType.Dart))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseCoupeJarret))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCoupeJarret, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCoupeJarret, 0);
        }
      }
    }
  }
}
