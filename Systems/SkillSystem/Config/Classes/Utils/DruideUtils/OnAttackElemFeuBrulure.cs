using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void OnAttackElemFeuBrulure(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (CreatureUtils.GetSavingThrowResult(target, Ability.Constitution, onAttack.Attacker, 13) == SavingThrowResult.Failure)
            target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Brulure, NwTimeSpan.FromRounds(2));

          break;
      }
    }
  }
}
