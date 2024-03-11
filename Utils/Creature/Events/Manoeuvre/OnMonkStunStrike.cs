using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnMonkStunStrike(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target || onAttack.IsRangedAttack)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          SpellConfig.SavingThrowFeedback feedback = new();
          int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Wisdom);
          int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;
          int advantage = GetCreatureAbilityAdvantage(target, Ability.Wisdom);
          int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Wisdom, tirDC, advantage, feedback);
          bool saveFailed = totalSave < tirDC;

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Frappe étourdissante", ColorConstants.Red, true);
          SpellUtils.SendSavingThrowFeedbackMessage(onAttack.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Wisdom);

          if (saveFailed)
            target.ApplyEffect(EffectDuration.Temporary, Effect.Stunned(), NwTimeSpan.FromRounds(1));

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnMonkStunStrike;

          break;
      }
    }
  }
}
