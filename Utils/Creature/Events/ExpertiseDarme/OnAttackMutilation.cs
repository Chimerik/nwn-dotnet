﻿using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackMutilation(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is NwCreature target)
      {
        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.CriticalHit:
          case AttackResult.AutomaticHit:

            NwItem weapon;

            switch (onAttack.WeaponAttackType)
            {
              case WeaponAttackType.MainHand:
              case WeaponAttackType.HastedAttack: weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand); break;
              case WeaponAttackType.Offhand: weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.LeftHand); break;
              default: return;
            }

            if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(onAttack.Attacker, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe, BaseItemType.Battleaxe, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Scythe))
            {
              int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, NativeUtils.GetAttackAbility(onAttack.Attacker, onAttack.IsRangedAttack, weapon));

              if (GetSavingThrowResult(target, Ability.Constitution, onAttack.Attacker, spellDC) == SavingThrowResult.Failure)
              {
                EffectSystem.Mutilation(target);
              }

              onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(onAttack.Attacker, 60, CustomSkill.ExpertiseMutilation), NwTimeSpan.FromRounds(10));

              await NwTask.NextFrame();
              onAttack.Attacker.OnCreatureAttack -= OnAttackMutilation;
            }

            break;
        }
      }
    }
  }
}
