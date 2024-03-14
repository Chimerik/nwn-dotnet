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
          int DC = 8 + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;
          int advantage = GetCreatureAbilityAdvantage(target, Ability.Wisdom);
          int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Wisdom, DC, advantage, feedback);
          bool saveFailed = totalSave < DC;

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Frappe étourdissante", StringUtils.gold, true, true);
          SpellUtils.SendSavingThrowFeedbackMessage(onAttack.Attacker, target, feedback, advantage, DC, totalSave, saveFailed, Ability.Wisdom);

          if (saveFailed)
            target.ApplyEffect(EffectDuration.Temporary, Effect.Stunned(), NwTimeSpan.FromRounds(1));

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnMonkStunStrike;

          break;
      }
    }
  }
}
