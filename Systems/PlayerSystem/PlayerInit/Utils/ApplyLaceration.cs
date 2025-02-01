using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyLaceration()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseLaceration))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipLaceration;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipLaceration;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipLaceration;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipLaceration;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.TwoBladedSword, BaseItemType.Bastardsword, BaseItemType.Katana, BaseItemType.Longsword, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
        || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseLaceration))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseLaceration, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseLaceration, 0);
        }
      }
    }
  }
}
