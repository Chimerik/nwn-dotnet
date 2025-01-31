using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

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

          if(!onAttack.Attacker.ActiveEffects.Any(e => e.Tag == EffectSystem.AttaquesEtudieesEffectTag))
            NWScript.AssignCommand(target, () => onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.AttaquesEtudiees, NwTimeSpan.FromRounds(1)));

          break;
      }
    }
  }
}
