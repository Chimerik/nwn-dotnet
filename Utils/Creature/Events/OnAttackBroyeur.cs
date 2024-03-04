using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnAttackBroyeur(OnCreatureAttack onAttack)
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

      if (onAttack.AttackResult == AttackResult.CriticalHit // Critical Hit
        && !onAttack.IsRangedAttack
        && weapon is not null
        && onAttack.Attacker.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Broyeur))
        && weapon.BaseItem.WeaponType.Any(d => d == Anvil.API.DamageType.Bludgeoning))
        onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.BroyeurEffect, NwTimeSpan.FromRounds(1));
    }
  }
}
