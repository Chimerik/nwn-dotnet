using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPreparation()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertisePreparation))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipPreparation;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipPreparation;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipPreparation;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipPreparation;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.Battleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Katana, BaseItemType.Scythe))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertisePreparation))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertisePreparation, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertisePreparation, 0);
        }
      }
    }
  }
}
