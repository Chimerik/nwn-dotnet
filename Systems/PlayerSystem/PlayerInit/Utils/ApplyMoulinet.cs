using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMoulinet()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseMoulinet))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipMoulinet;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipMoulinet;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipMoulinet;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipMoulinet;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Rapier, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
          || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Rapier, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseMoulinet))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMoulinet, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMoulinet, 0);
        }
      }
    }
  }
}
