using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackDestabiliser(OnCreatureAttack onAttack)
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

            if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Quarterstaff, BaseItemType.MagicStaff, BaseItemType.Whip))
            {
              int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, NativeUtils.GetAttackAbility(onAttack.Attacker, onAttack.IsRangedAttack, weapon));

              if (GetSavingThrow(onAttack.Attacker, target, Ability.Dexterity, spellDC) == SavingThrowResult.Failure)
              {
                EffectSystem.ApplyKnockdown(target, onAttack.Attacker, Ability.Dexterity, Ability.Dexterity, EffectSystem.Destabilisation, true);
                onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(onAttack.Attacker, 60, CustomSkill.ExpertiseDestabiliser), NwTimeSpan.FromRounds(10));
              }

              await NwTask.NextFrame();
              onAttack.Attacker.OnCreatureAttack -= OnAttackDestabiliser;
            }

            break;
        }
      }
    }
  }
}
