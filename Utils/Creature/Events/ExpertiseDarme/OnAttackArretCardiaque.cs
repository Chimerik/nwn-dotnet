﻿using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackArretCardiaque(OnCreatureAttack onAttack)
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

            if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(onAttack.Attacker, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightMace, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.HeavyFlail))
            {
              int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, NativeUtils.GetAttackAbility(onAttack.Attacker, onAttack.IsRangedAttack, weapon));

              if (GetSavingThrowResult(target, Ability.Constitution, onAttack.Attacker, spellDC) == SavingThrowResult.Failure)
              {
                EffectSystem.ArretCardiaque(target);
              }

              onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(onAttack.Attacker, 60, CustomSkill.ExpertiseArretCardiaque), NwTimeSpan.FromRounds(10));

              await NwTask.NextFrame();
              onAttack.Attacker.OnCreatureAttack -= OnAttackArretCardiaque;
            }

            break;
        }
      }
    }
  }
}
