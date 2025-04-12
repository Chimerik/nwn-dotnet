using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPoidsPlumeStyle()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FightingStylePoidsPlume))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipPoidsPlumeStyle;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipPoidsPlumeStyle;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipPoidsPlumeStyle;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipPoidsPlumeStyle;

          var armor = oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);
          var rightWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var leftWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((armor is null || !ItemUtils.IsMediumOrHeavyArmor(armor))
            && (rightWeapon is null || ItemUtils.IsLightWeapon(rightWeapon.BaseItem, oid.LoginCreature.Size))
            && (leftWeapon is null || ItemUtils.IsLightWeapon(leftWeapon.BaseItem, oid.LoginCreature.Size)))
          {
            EffectUtils.RemoveTaggedEffect(oid.LoginCreature, EffectSystem.PoidsPlumeStyleEffectTag);
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.PoidsPlumeStyle);
          }
          else
            EffectUtils.RemoveTaggedEffect(oid.LoginCreature, EffectSystem.PoidsPlumeStyleEffectTag);
        }
      }
    }
  }
}
