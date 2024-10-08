﻿using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnAttackSpiderPoisonBite(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target || target.IsImmuneTo(ImmunityType.Poison) || target.ActiveEffects.Any(e => e.Tag == EffectSystem.PoisonEffectTag))
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          NwCreature master = onAttack.Attacker.Master is null ? onAttack.Attacker : onAttack.Attacker.Master;
          int spellDC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(master) + onAttack.Attacker.GetAbilityModifier(Ability.Wisdom);

          if (CreatureUtils.GetSavingThrow(onAttack.Attacker, target, Ability.Constitution, spellDC) == SavingThrowResult.Failure)
          {
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Poison, NwTimeSpan.FromRounds(2)));
          }

          break;
      }
    }
  }
}
