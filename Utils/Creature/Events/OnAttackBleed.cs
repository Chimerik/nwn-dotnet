using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackApplyBleed(OnCreatureAttack onAttack)
    {
      if(onAttack.Target.GetObjectVariable<LocalVariableInt>(ApplyBleedVariable).HasValue)
      {
        onAttack.Target.ApplyEffect(EffectDuration.Temporary, EffectSystem.saignementEffect, NwTimeSpan.FromRounds(2));
        onAttack.Target.GetObjectVariable<LocalVariableInt>(ApplyBleedVariable).Delete();

        if(onAttack.Attacker.GetObjectVariable<LocalVariableInt>(TigerAspectBleedVariable).HasValue)
        {
          onAttack.Attacker.GetObjectVariable<LocalVariableInt>(TigerAspectBleedVariable).Value -= 1;

          if(onAttack.Attacker.GetObjectVariable<LocalVariableInt>(TigerAspectBleedVariable).Value < 1)
          {
            onAttack.Attacker.GetObjectVariable<LocalVariableInt>(TigerAspectBleedVariable).Delete();
            onAttack.Attacker.OnCreatureAttack -= OnAttackApplyBleed;
          }
        }
      }
    }
  }
}
