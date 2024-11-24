using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnAttackMorsurePlongeante(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

            EffectSystem.ApplyKnockdown(target, onAttack.Attacker);
            StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Morsure Plongeante", StringUtils.gold);

          break;
      }
    }
  }
}
