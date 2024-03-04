using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnAttackPourfendeur(OnCreatureAttack onAttack)
    {
      NwItem weapon = null;

      switch (onAttack.WeaponAttackType)
      {
        case WeaponAttackType.MainHand:
        case WeaponAttackType.HastedAttack:
          weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);
          break;
        case WeaponAttackType.Offhand:
          weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.LeftHand);
          break;
        case WeaponAttackType.CreatureRight:
          weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.CreatureRightWeapon);
          break;
        case WeaponAttackType.CreatureLeft:
          weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
          break;
        case WeaponAttackType.CreatureBite:
          weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.CreatureBiteWeapon);
          break;
      }

      if(weapon is not null && weapon.BaseItem.WeaponType.Any(d => d == DamageType.Slashing))
        onAttack.Attacker.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_SLASH").Value = 1;

      if (onAttack.AttackResult == AttackResult.CriticalHit)
        onAttack.Attacker.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_CRIT").Value = 1;
    }
  }
}
