using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMutilation()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseMutilation))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipMutilation;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipMutilation;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipMutilation;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipMutilation;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe, BaseItemType.Battleaxe, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Scythe))
          || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseMutilation))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMutilation, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMutilation, 0);
        }
      }
    }
  }
}
