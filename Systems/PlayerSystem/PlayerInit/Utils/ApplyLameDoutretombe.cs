using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyLameDoutretombe()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.LameDoutretombe))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipApplyLameDoutretombe;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipRemoveLameDoutretombe;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipApplyLameDoutretombe;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipRemoveLameDoutretombe;

          if (oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType == BaseItemType.TwoBladedSword
            && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.LameDoutretombeEffectTag))
          {
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.lameDoutretombe);
            oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand).GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;
          }
        }
      }
    }
  }
}
