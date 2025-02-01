using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMonkUnarmoredDefence()
      {
        if (oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Monk))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipMonkUnarmoredDefence;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipMonkUnarmoredDefence;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipMonkUnarmoredDefence;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipMonkUnarmoredDefence;

          NwItem armor = oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);
          NwItem shield = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if (armor is null || armor.BaseACValue < 1)
          {
            if (shield is not null)
            {
              switch (shield.BaseItem.ItemType)
              {
                case BaseItemType.SmallShield:
                case BaseItemType.LargeShield:
                case BaseItemType.TowerShield:
                  return;
              }
            }

            oid.LoginCreature.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckMonkUnarmoredDefence;
            oid.LoginCreature.OnHeartbeat += CreatureUtils.OnHeartBeatCheckMonkUnarmoredDefence;
            EffectSystem.ApplyMonkUnarmoredDefenseEffect(oid.LoginCreature);

            if (oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_MONK_SPEED_DISABLED").HasNothing
            &&  oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 1)
            && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkSpeedEffectTag))
            {
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkSpeedEffect(oid.LoginCreature.Classes.FirstOrDefault(c => c.Class.Id == CustomClass.Monk).Level));
            }
          }
        }
      }
    }
  }
}
