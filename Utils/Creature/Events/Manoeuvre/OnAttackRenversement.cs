using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackRenversement(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;
      
      switch (onAttack.AttackResult)
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

          LogUtils.LogMessage($"--- {onAttack.Attacker.Name} renversement contre {target.Name} ---", LogUtils.LogType.Combat);
          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Renversement", ColorConstants.Red, true, true);
          SpellUtils.SendSavingThrowFeedbackMessage(onAttack.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Strength);

          if (saveFailed)
            EffectSystem.ApplyKnockdown(target, CreatureSize.Large, 1);

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackRenversement;

          break;
      }
    }
  }
}
