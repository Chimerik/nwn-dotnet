using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackBalayage(OnCreatureAttack onAttack)
    {
      onAttack.Attacker.OnCreatureAttack -= OnAttackBalayage;

      NwCreature target = onAttack.Attacker.GetNearestCreatures(CreatureTypeFilter.Alive(true), CreatureTypeFilter.Perception(PerceptionType.Seen),
        CreatureTypeFilter.Reputation(ReputationType.Enemy)).FirstOrDefault(t => t != onAttack.Target);

      if (target is not null && target.DistanceSquared(onAttack.Attacker) < 9)
        onAttack.Attacker.GetObjectVariable<LocalVariableObject<NwCreature>>(ManoeuvreBalayageTargetVariable).Value = target;
    }
  }
}
