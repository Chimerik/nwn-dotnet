using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static void OnAttackQuasitPoison(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          EffectSystem.ApplyPoison(target, onAttack.Attacker, NwTimeSpan.FromRounds(1), Ability.Constitution, Ability.Wisdom, noSave: true);

          break;
      }
    }
  }
}
