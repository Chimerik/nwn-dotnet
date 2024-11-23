using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static async void OnAttackFrappeRedoutable(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature targetCreature)
        return;

      NwCreature caster = onAttack.Attacker;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          int nbDice = onAttack.AttackResult == AttackResult.CriticalHit ? 4 : 2;
          int damageDice = caster.KnowsFeat((Feat)CustomSkill.TraqueurRafale) ? 8 : 6;

          NWScript.AssignCommand(caster, () => targetCreature.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, damageDice, nbDice), CustomDamageType.Psychic)));

          NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary,
            EffectSystem.Cooldown(caster, 6, CustomSkill.ProfondeursFrappeRedoutable), NwTimeSpan.FromRounds(1)));

          await NwTask.NextFrame();
          caster.OnCreatureAttack -= OnAttackFrappeRedoutable;
          EffectUtils.RemoveTaggedEffect(caster, EffectSystem.FrappeRedoutableEffectTag);

          break;
      }
    }
  }
}
