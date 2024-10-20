using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void OnAttackElemEauChill(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (CreatureUtils.GetSavingThrow(onAttack.Attacker, target, Ability.Constitution, 13) == SavingThrowResult.Failure)
            target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Chill, NwTimeSpan.FromRounds(2));

          break;
      }
    }
  }
}
