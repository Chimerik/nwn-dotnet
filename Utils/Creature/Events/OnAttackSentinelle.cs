using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackSentinelle(OnCreatureAttack onAttack)
    {
      if (onAttack.AttackType != 65002 && onAttack.Attacker.GetObjectVariable<LocalVariableInt>(SentinelleOpportunityVariable).HasNothing)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.AutomaticHit:
        case AttackResult.CriticalHit:
        case AttackResult.DevastatingCritical:
        case AttackResult.Resisted:
          onAttack.Target.ApplyEffect(EffectDuration.Temporary, EffectSystem.sentinelleEffect, NwTimeSpan.FromRounds(1));
          break;
      }

      onAttack.Attacker.GetObjectVariable<LocalVariableInt>(SentinelleOpportunityVariable).Delete();
    }
  }
}
