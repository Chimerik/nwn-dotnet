﻿using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static async void OnAttackBriseurDeHordes(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature targetCreature)
        return;

      NwCreature caster = onAttack.Attacker;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BriseurDeHordesVariable).HasValue)
          {
            caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BriseurDeHordesVariable).Delete();

            NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Attacker.ApplyEffect(EffectDuration.Temporary,
              EffectSystem.Cooldown(onAttack.Attacker, 6, CustomSkill.ChasseurProie, NwSpell.FromSpellId(CustomSpell.BriseurDeHordes)), NwTimeSpan.FromRounds(1)));

            await NwTask.NextFrame();
            onAttack.Attacker.OnCreatureAttack -= OnAttackBriseurDeHordes;
          }

          break;
      }
    }
  }
}
