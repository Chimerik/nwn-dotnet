using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnPoidsPlumeStyle(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      var armor = player.oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);
      var rightWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var leftWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((armor is null || !ItemUtils.IsMediumOrHeavyArmor(armor))
        && (rightWeapon is null || ItemUtils.IsLightWeapon(rightWeapon.BaseItem, player.oid.LoginCreature.Size))
        && (leftWeapon is null || ItemUtils.IsLightWeapon(leftWeapon.BaseItem, player.oid.LoginCreature.Size)))
      {
        EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.PoidsPlumeStyleEffectTag);
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.PoidsPlumeStyle);
      }

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipPoidsPlumeStyle;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipPoidsPlumeStyle;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipPoidsPlumeStyle;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipPoidsPlumeStyle;

      return true;
    }
  }
}
