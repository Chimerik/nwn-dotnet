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

            if (onAttack.Target is NwCreature target)
            {
              SpellConfig.SavingThrowFeedback feedback = new();

              Ability saveAbility = target.GetAbilityModifier(Ability.Strength) > target.GetAbilityModifier(Ability.Dexterity)
                ? Ability.Strength : Ability.Dexterity;

              int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Wisdom);
              int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;
              StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Technique de la paume", StringUtils.gold, true, true);
   
              if (GetSavingThrow(onAttack.Attacker, target, saveAbility, DC) == SavingThrowResult.Failure)
                EffectSystem.ApplyKnockdown(target, CreatureSize.Large, 2);
            }

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
