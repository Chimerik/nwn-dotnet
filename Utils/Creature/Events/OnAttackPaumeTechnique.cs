using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackPaumeTechnique(OnCreatureAttack onAttack)
    {
      if (!onAttack.IsRangedAttack)
      {
        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.AutomaticHit:
          case AttackResult.CriticalHit:

            if (onAttack.Target is NwCreature target && target.Size < CreatureSize.Huge)
            {
              SpellConfig.SavingThrowFeedback feedback = new();

              Ability saveAbility = target.GetAbilityModifier(Ability.Strength) > target.GetAbilityModifier(Ability.Dexterity)
                ? Ability.Strength : Ability.Dexterity;

              int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Wisdom);
              int DC = 8 + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;
              int advantage = GetCreatureAbilityAdvantage(target, saveAbility);
              int totalSave = SpellUtils.GetSavingThrowRoll(target, saveAbility, DC, advantage, feedback);
              bool saveFailed = totalSave < DC;

              StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Technique de la paume", StringUtils.gold, true, true);
              SpellUtils.SendSavingThrowFeedbackMessage(onAttack.Attacker, target, feedback, advantage, DC, totalSave, saveFailed, saveAbility);

              if (saveFailed)
                target.ApplyEffect(EffectDuration.Temporary, EffectSystem.knockdown, NwTimeSpan.FromRounds(2));
            }
            else
              onAttack.Attacker?.LoginPlayer.SendServerMessage($"Impossible de renverser {onAttack.Target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Red);

            break;
        }
      }

      onAttack.Attacker.GetObjectVariable<LocalVariableInt>(MonkPaumeTechniqueVariable).Value -= 1;

      if (onAttack.Attacker.GetObjectVariable<LocalVariableInt>(MonkPaumeTechniqueVariable).Value < 1)
      {
        onAttack.Attacker.GetObjectVariable<LocalVariableInt>(MonkPaumeTechniqueVariable).Delete();

        await NwTask.NextFrame();
        onAttack.Attacker.OnCreatureAttack -= OnAttackPaumeTechnique;
      }
    }
  }
}
