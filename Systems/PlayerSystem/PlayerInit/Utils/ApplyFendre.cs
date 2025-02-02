using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFendre()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseFendre))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipFendre;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipFendre;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipFendre;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipFendre;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Battleaxe, BaseItemType.Doubleaxe, BaseItemType.Greatsword, BaseItemType.TwoBladedSword, BaseItemType.Greataxe, BaseItemType.DwarvenWaraxe, BaseItemType.Halberd, BaseItemType.Scythe))
            || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Battleaxe, BaseItemType.DwarvenWaraxe)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseFendre))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFendre, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFendre, 0);
        }
      }
    }
  }
}
