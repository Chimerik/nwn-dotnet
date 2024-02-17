using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyUnarmoredDefence()
      {
        if (oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level > 0))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipUnarmoredDefence;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipUnarmoredDefence;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipUnarmoredDefence;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipUnarmoredDefence;

          NwItem armor = oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);

          if (armor is null || armor.BaseACValue < 1)
          {
            oid.LoginCreature.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
            oid.LoginCreature.OnHeartbeat += CreatureUtils.OnHeartBeatCheckUnarmoredDefence;

            if (oid.LoginCreature.GetAbilityModifier(Ability.Constitution) > 0)
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetUnarmoredDefenseEffect(oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));
          }
        }
      }
    }
  }
}
