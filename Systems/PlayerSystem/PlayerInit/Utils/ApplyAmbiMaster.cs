using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAmbiMaster()
      {
        if (learnableSkills.TryGetValue(CustomSkill.AmbiMaster, out var protection) && protection.currentLevel > 0)
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipApplyAmbiMaster;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipRemoveAmbiMaster;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipApplyAmbiMaster;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipRemoveAmbiMaster;

          if(ItemUtils.IsWeapon(oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem) && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ambiMaster);
        }
      }
    }
  }
}
