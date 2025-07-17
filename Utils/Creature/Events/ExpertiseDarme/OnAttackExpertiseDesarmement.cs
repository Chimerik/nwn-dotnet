using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackExpertiseDesarmement(OnCreatureAttack onAttack)
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

            if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(onAttack.Attacker, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Whip))
            {
              int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, NativeUtils.GetAttackAbility(onAttack.Attacker, onAttack.IsRangedAttack, weapon));

              if (GetSavingThrowResult(target, Ability.Strength, onAttack.Attacker, spellDC) == SavingThrowResult.Failure)
              {
                NwItem disarmedWeapon = target.GetItemInSlot(InventorySlot.RightHand);

                if (disarmedWeapon is not null)
                {
                  if (disarmedWeapon.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").HasNothing)
                  {
                    target.RunUnequip(disarmedWeapon);

                    target.OnItemEquip -= ItemSystem.OnEquipDesarmement;
                    target.OnItemEquip += ItemSystem.OnEquipDesarmement;

                    target.ApplyEffect(EffectDuration.Temporary, EffectSystem.warMasterDesarmement, NwTimeSpan.FromRounds(1));
                  }
                  else
                    onAttack.Attacker?.LoginPlayer.SendServerMessage($"L'arme de {target.Name.ColorString(ColorConstants.Cyan)} est liée et ne peut être désarmée");
                }                
              }

              onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(onAttack.Attacker, 60, CustomSkill.ExpertiseDesarmement), NwTimeSpan.FromRounds(10));

              await NwTask.NextFrame();
              onAttack.Attacker.OnCreatureAttack -= OnAttackExpertiseDesarmement;
            }

            break;
        }
      }
    }
  }
}
