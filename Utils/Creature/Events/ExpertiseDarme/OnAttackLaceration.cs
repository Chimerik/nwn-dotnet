﻿using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackLaceration(OnCreatureAttack onAttack)
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

            if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.TwoBladedSword, BaseItemType.Bastardsword, 
              BaseItemType.Katana, BaseItemType.Longsword, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
            {
              int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, NativeUtils.GetAttackAbility(onAttack.Attacker, onAttack.IsRangedAttack, weapon));

              if (GetSavingThrow(onAttack.Attacker, target, Ability.Constitution, spellDC) == SavingThrowResult.Failure)
              {
                EffectSystem.Laceration(target);
                onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(onAttack.Attacker, 60, CustomSkill.ExpertiseLaceration), NwTimeSpan.FromRounds(10));
              }

              await NwTask.NextFrame();
              onAttack.Attacker.OnCreatureAttack -= OnAttackLaceration;
            }

            break;
        }
      }
    }
  }
}
