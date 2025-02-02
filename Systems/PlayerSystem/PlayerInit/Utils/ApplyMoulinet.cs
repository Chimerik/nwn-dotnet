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
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipMoulinet;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipMoulinet;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipMoulinet;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipMoulinet;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Rapier, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
          || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Rapier, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
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
