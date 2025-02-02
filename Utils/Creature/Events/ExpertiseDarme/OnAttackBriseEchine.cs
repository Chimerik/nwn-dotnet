using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackBriseEchine(OnCreatureAttack onAttack)
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
              default: return;
            }

            if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(onAttack.Attacker, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.DireMace, BaseItemType.Warhammer))
            {
              int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, NativeUtils.GetAttackAbility(onAttack.Attacker, onAttack.IsRangedAttack, weapon));

              if (GetSavingThrow(onAttack.Attacker, target, Ability.Constitution, spellDC) == SavingThrowResult.Failure)
              {
                EffectSystem.ApplyKnockdown(target, onAttack.Attacker, Ability.Strength, Ability.Constitution, EffectSystem.Destabilisation, true);
                onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(onAttack.Attacker, 60, CustomSkill.ExpertiseBriseEchine), NwTimeSpan.FromRounds(10));
              }

              await NwTask.NextFrame();
              onAttack.Attacker.OnCreatureAttack -= OnAttackBriseEchine;
            }

            break;
        }
      }
    }
  }
}
