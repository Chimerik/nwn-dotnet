using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMonkUnarmoredDefence()
      {
        if (oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Monk))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipMonkUnarmoredDefence;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipMonkUnarmoredDefence;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipMonkUnarmoredDefence;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipMonkUnarmoredDefence;

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

            if (oid.LoginCreature.GetAbilityModifier(Ability.Wisdom) > 0)
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkUnarmoredDefenseEffect(oid.LoginCreature.GetAbilityModifier(Ability.Wisdom)));

            if (oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Monk && c.Level > 1)
            && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkSpeedEffectTag))
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkSpeedEffect(oid.LoginCreature.Classes.FirstOrDefault(c => c.Class.ClassType == ClassType.Monk).Level));
          }
        }
      }
    }
  }
}
