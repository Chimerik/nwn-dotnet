using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void OnAttackElemTerreKnockdown(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target || target.IsImmuneTo(ImmunityType.Knockdown))
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (CreatureUtils.GetSavingThrowResult(target, Ability.Constitution, onAttack.Attacker, 13) == SavingThrowResult.Failure)
            EffectSystem.ApplyKnockdown(target, onAttack.Attacker);

          break;
      }
    }
  }
}
