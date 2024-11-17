using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void OnAttackAttaquesEtudiees(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Miss:
        case AttackResult.Parried:
        case AttackResult.Concealed:
        case AttackResult.MissChance:

          NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.AttaquesEtudiees, NwTimeSpan.FromRounds(1)));

          break;
      }
    }
  }
}
