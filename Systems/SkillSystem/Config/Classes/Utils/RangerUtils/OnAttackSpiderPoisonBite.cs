using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnAttackSpiderPoisonBite(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          NwCreature master = onAttack.Attacker.Master is null ? onAttack.Attacker : onAttack.Attacker.Master;
          EffectSystem.ApplyPoison(target, master, NwTimeSpan.FromRounds(2), Ability.Constitution, Ability.Wisdom);

          break;
      }
    }
  }
}
