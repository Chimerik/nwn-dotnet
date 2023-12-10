using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackBroyeur(OnCreatureAttack onAttack)
    {
      NwItem weapon = null;

      switch(onAttack.AttackType)
      {
        case 1:
        case 6:
          weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);
          break;
        case 2:
          weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.LeftHand);
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
