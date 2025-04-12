using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;
using System;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static async void OnAttackPourfendeur3(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.MarqueDuChasseurTag && e.Creator == onAttack.Attacker))
          {
            if(CreatureUtils.GetSavingThrow(onAttack.Attacker, target, Ability.Constitution, SpellUtils.GetCasterSpellDC(onAttack.Attacker, Ability.Wisdom)) == SavingThrowResult.Failure)
            {
              target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpBlindDeafM));
              NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Silence(), Effect.Blindness(), Effect.VisualEffect(VfxType.DurMindAffectingDisabled)), NwTimeSpan.FromRounds(1)));
            }

            onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(onAttack.Attacker, 6, CustomSkill.Pourfendeur3), TimeSpan.FromSeconds(5.8));

            await NwTask.NextFrame();
            onAttack.Attacker.OnCreatureAttack -= OnAttackPourfendeur3;
          }

          break;
      }
    }
  }
}
