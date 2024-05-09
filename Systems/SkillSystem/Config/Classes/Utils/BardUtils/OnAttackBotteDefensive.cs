using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BardUtils
  {
    public static async void OnAttackBotteDefensive(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:
          
          onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetBotteDefensiveEffect(onAttack.Attacker.GetObjectVariable<LocalVariableInt>(FeatSystem.BotteDefensiveVariable).Value), NwTimeSpan.FromRounds(1));

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Botte Défensive", StringUtils.gold, true);
          
          break;
      }

      onAttack.Attacker.GetObjectVariable<LocalVariableInt>(FeatSystem.BotteDefensiveVariable).Delete();

      await NwTask.NextFrame();
      onAttack.Attacker.OnCreatureAttack -= OnAttackBotteDefensive;
    }
  }
}
