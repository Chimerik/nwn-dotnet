using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMonkUnarmoredDefence(PlayerSystem.Player player, int customSkillId)
    {
      if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MonkUnarmoredDefence))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MonkUnarmoredDefence);

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipMonkUnarmoredDefence;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipMonkUnarmoredDefence;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipMonkUnarmoredDefence;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipMonkUnarmoredDefence;

      NwItem armor = player.oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);
      NwItem shield = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if (armor is null || armor.BaseACValue < 1)
      {
        if(shield is not null)
        {
          switch (shield.BaseItem.ItemType)
          {
            case BaseItemType.SmallShield:
            case BaseItemType.LargeShield:
            case BaseItemType.TowerShield:
              return true;
          }
        }

        player.oid.LoginCreature.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckMonkUnarmoredDefence;
        player.oid.LoginCreature.OnHeartbeat += CreatureUtils.OnHeartBeatCheckMonkUnarmoredDefence;
        EffectSystem.ApplyMonkUnarmoredDefenseEffect(player.oid.LoginCreature);
      }

      return true;
    }
  }
}
