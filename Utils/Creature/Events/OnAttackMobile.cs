using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackMobile(OnCreatureAttack onAttack)
    {
      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:
          NWScript.AssignCommand(onAttack.Attacker, () 
            => onAttack.Target.ApplyEffect(EffectDuration.Temporary, EffectSystem.mobileDebuff, NwTimeSpan.FromRounds(1)));
          break;
      }
    }
  }
}
