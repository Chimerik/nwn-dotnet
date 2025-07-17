using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System;

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

          if (CreatureUtils.GetSavingThrowResult(targetCreature, Ability.Charisma, caster, spellDC) == SavingThrowResult.Failure)
            ApplyTraverseeInfernale(caster, targetCreature);

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackTraverseeInfernale;
          
          break;
      }
    }
    private static async void ApplyTraverseeInfernale(NwCreature attacker, NwCreature target)
    {
      target.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonGate));

      await NwTask.Delay(TimeSpan.FromSeconds(0.8));
      NWScript.AssignCommand(attacker, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.TraverseeInfernale(target), NwTimeSpan.FromRounds(1)));
    }
  }
}
