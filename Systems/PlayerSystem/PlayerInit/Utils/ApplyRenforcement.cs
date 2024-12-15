using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyRenforcement()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseRenforcement))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipRenforcement;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipRenforcement;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipRenforcement;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipRenforcement;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.ShortSpear, BaseItemType.Halberd, BaseItemType.TwoBladedSword))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseRenforcement))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseRenforcement, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseRenforcement, 0);
        }
      }
    }
  }
}
