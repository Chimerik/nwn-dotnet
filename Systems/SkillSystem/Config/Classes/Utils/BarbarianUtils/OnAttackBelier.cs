using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static void OnAttackBelier(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit: EffectSystem.ApplyKnockdown(target, CreatureSize.Large, 2); break;
      }
    }
  }
}
