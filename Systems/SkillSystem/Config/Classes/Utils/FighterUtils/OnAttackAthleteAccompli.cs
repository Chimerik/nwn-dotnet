using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void OnAttackAthleteAccompli(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.CriticalHit:

          NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.AthleteAccompli, NwTimeSpan.FromRounds(1)));

          break;
      }
    }
  }
}
