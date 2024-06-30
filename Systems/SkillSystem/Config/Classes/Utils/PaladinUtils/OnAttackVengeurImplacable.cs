using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static void OnAttackVengeurImplacable(OnCreatureAttack onAttack)
    {
      if (onAttack.AttackType != 65002 || onAttack.Target is not NwCreature target 
        || onAttack.Attacker.ActiveEffects.Any(e => e.Tag == EffectSystem.VengeurImplacableEffectTag))
        return;

        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.CriticalHit:
          case AttackResult.AutomaticHit:

              onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.VengeurImplacable, NwTimeSpan.FromRounds(1));

            break;
        }
    }
  }
}
