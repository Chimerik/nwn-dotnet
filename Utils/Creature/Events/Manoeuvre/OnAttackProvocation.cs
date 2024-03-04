using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using NWN.Core;
using NativeUtils = NWN.Systems.NativeUtils;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackProvocation(OnCreatureAttack onAttack)
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
          int advantage = GetCreatureAbilityAdvantage(target, Ability.Wisdom);
          int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Wisdom, tirDC, advantage, feedback);
          bool saveFailed = totalSave < tirDC;

          SpellUtils.SendSavingThrowFeedbackMessage(onAttack.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Wisdom);

          if (saveFailed)
          {
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary,
            EffectSystem.provocation, NwTimeSpan.FromRounds(1)));
          }

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Attaque Menaçante", ColorConstants.Red, true);

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackProvocation;

          break;
      }
    }
  }
}
