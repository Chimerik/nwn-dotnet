using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackDesarmement(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Strength) > onAttack.Attacker.GetAbilityModifier(Ability.Dexterity) ? onAttack.Attacker.GetAbilityModifier(Ability.Strength) : onAttack.Attacker.GetAbilityModifier(Ability.Dexterity);
          int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;

          LogUtils.LogMessage($"--- {onAttack.Attacker.Name} désarmement contre {target.Name} ---", LogUtils.LogType.Combat);
          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Désarmement", ColorConstants.Red, true, true);

          if (GetSavingThrow(onAttack.Attacker, target, Ability.Strength, DC) == SavingThrowResult.Failure)
          {
            NwItem weapon = target.GetItemInSlot(InventorySlot.RightHand);

            if (weapon is not null)
            {
              if (weapon.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").HasNothing || EffectUtils.IsIncapacitated(onAttack.Attacker))
              {
                target.RunUnequip(weapon);

                target.OnItemEquip -= ItemSystem.OnEquipDesarmement;
                target.OnItemEquip += ItemSystem.OnEquipDesarmement;

                target.ApplyEffect(EffectDuration.Temporary, EffectSystem.warMasterDesarmement, NwTimeSpan.FromRounds(1));
              }
              else
                onAttack.Attacker?.LoginPlayer.SendServerMessage($"L'arme de {target.Name.ColorString(ColorConstants.Cyan)} est liée et ne peut être désarmée");
            }
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackDesarmement;

          break;
      }
    }
  }
}
