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
        if (oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipUnarmoredDefence;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipUnarmoredDefence;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipUnarmoredDefence;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipUnarmoredDefence;

          NwItem armor = oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);

          if (armor is null || armor.BaseACValue < 1)
          {
            oid.LoginCreature.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
            oid.LoginCreature.OnHeartbeat += CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
            EffectSystem.ApplyUnarmoredDefenseEffect(oid.LoginCreature);

            if (oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Barbarian && c.Level > 4)
            && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianSpeedEffectTag))
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.BarbarianSpeed);
          }
        }
      }
    }
  }
}
