using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using NativeUtils = NWN.Systems.NativeUtils;

namespace NWN
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

          SpellConfig.SavingThrowFeedback feedback = new();
          int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Strength) > onAttack.Attacker.GetAbilityModifier(Ability.Dexterity) ? onAttack.Attacker.GetAbilityModifier(Ability.Strength) : onAttack.Attacker.GetAbilityModifier(Ability.Dexterity);
          int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;
          int advantage = GetCreatureAbilityAdvantage(target, Ability.Strength);
          int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Strength, tirDC, advantage, feedback);
          bool saveFailed = totalSave < tirDC;

          SpellUtils.SendSavingThrowFeedbackMessage(onAttack.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Strength);

          if (saveFailed)
          {
            NwItem weapon = target.GetItemInSlot(InventorySlot.RightHand);

            if (weapon is not null)
              target.RunUnequip(weapon);

            target.OnItemEquip -= ItemSystem.OnEquipDesarmement;
            target.OnItemEquip += ItemSystem.OnEquipDesarmement;

            target.ApplyEffect(EffectDuration.Temporary, EffectSystem.warMasterDesarmement, NwTimeSpan.FromRounds(1));
          }

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Désarmement", ColorConstants.Red, true);

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackDesarmement;

          break;
      }
    }
  }
}
