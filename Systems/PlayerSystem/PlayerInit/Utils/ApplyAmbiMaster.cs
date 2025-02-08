using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAmbiMaster()
      {
        if (oid.LoginCreature.KnowsFeat(Feat.Ambidexterity))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipApplyAmbiMaster;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipRemoveAmbiMaster;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipApplyAmbiMaster;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipRemoveAmbiMaster;

          if (ItemUtils.IsMeleeWeapon(oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem)
        && ItemUtils.IsMeleeWeapon(oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem)
        && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ambiMaster);
        }
      }
    }
  }
}
