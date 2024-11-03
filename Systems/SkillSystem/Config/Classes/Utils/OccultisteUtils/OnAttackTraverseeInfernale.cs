using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static async void OnAttackTraverseeInfernale(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature targetCreature)
        return;

      NwCreature caster = onAttack.Attacker;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:
   
          EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.TraverseeInfernaleBuffEffectTag);
          int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Charisma);

          if (CreatureUtils.GetSavingThrow(caster, targetCreature, Ability.Charisma, spellDC) == SavingThrowResult.Failure)
          {
            NWScript.AssignCommand(onAttack.Attacker, () => targetCreature.ApplyEffect(EffectDuration.Temporary,
              EffectSystem.TraverseeInfernale(targetCreature), NwTimeSpan.FromRounds(1)));
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackTraverseeInfernale;
          
          break;
      }
    }
  }
}
