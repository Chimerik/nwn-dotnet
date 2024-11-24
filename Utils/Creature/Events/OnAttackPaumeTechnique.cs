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
              EffectSystem.ApplyKnockdown(target, onAttack.Attacker, Ability.Wisdom, Ability.Dexterity);
              StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Technique de la paume", StringUtils.gold, true, true);
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
