using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static async void OnAttackCorbeauAveuglement(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Aveuglement", StringUtils.gold);

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          onAttack.Target.ApplyEffect(EffectDuration.Temporary, Effect.Blindness(), NwTimeSpan.FromRounds(2));
          
          break;
      }

      await NwTask.NextFrame();
      onAttack.Attacker.OnCreatureAttack -= OnAttackCorbeauAveuglement;
    }
  }
}
