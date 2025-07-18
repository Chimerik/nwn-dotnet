﻿using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using NWN.Core;
using NativeUtils = NWN.Systems.NativeUtils;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackMenacante(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Attaque Menaçante", ColorConstants.Red, true);

          int attackerModifier = onAttack.Attacker.GetAbilityModifier(Ability.Strength) > onAttack.Attacker.GetAbilityModifier(Ability.Dexterity) ? onAttack.Attacker.GetAbilityModifier(Ability.Strength) : onAttack.Attacker.GetAbilityModifier(Ability.Dexterity);
          int spellDC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onAttack.Attacker) + attackerModifier;

          if(GetSavingThrowResult(target, Ability.Wisdom, onAttack.Attacker, spellDC) == SavingThrowResult.Failure)
          {
            EffectSystem.ApplyEffroi(target, onAttack.Attacker, NwTimeSpan.FromRounds(1), spellDC);
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackMenacante;

          break;
      }
    }
  }
}
