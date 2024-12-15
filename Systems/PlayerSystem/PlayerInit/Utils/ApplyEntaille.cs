using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyEntaille()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseEntaille))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipEntaille;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipEntaille;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipEntaille;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipEntaille;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Handaxe, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.LightHammer, BaseItemType.Kukri, BaseItemType.Sickle))
        || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Handaxe, BaseItemType.Scimitar, BaseItemType.LightHammer, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseEntaille))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseEntaille, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseEntaille, 0);
        }
      }
    }
  }
}
