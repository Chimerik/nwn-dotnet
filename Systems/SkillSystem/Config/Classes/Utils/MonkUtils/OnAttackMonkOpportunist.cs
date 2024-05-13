using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class MonkUtils
  {
    public static void OnAttackMonkOpportunist(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      if(!target.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkOpportunistEffectTag))
        NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.MonkOpportunist, NwTimeSpan.FromRounds(1)));
    }
  }
}
