using Anvil.API.Events;
using Anvil.API;
using NativeUtils = NWN.Systems.NativeUtils;
using NWN.Systems;
using NWN.Core;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void HandleTirEnvoutant(OnCreatureAttack onDamage)
    {
      int damage = onDamage.Attacker.GetClassInfo(NwClass.FromClassId(CustomClass.ArcaneArcher))?.Level < 18
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(damage, CustomDamageType.Psychic)));

      if (onDamage.Target is NwCreature target)
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(onDamage.Attacker) + onDamage.Attacker.GetAbilityModifier(Ability.Intelligence);
        int advantage = GetCreatureAbilityAdvantage(target, Ability.Wisdom);
        int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Wisdom, tirDC, advantage, feedback);
        bool saveFailed = totalSave < tirDC;

        SpellUtils.SendSavingThrowFeedbackMessage(onDamage.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Constitution);

        if (saveFailed)
        {
          if(target.IsLoginPlayerCharacter)
            NWScript.AssignCommand(onDamage.Attacker, () => target.ApplyEffect(EffectDuration.Temporary,
              EffectSystem.charmEffect, NwTimeSpan.FromRounds(1)));
          else
            NWScript.AssignCommand(onDamage.Attacker, () => target.ApplyEffect(EffectDuration.Temporary,
              Effect.Charmed(), NwTimeSpan.FromRounds(1)));
        }
      }

      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir Envoûtant", StringUtils.gold, true);
    }
  }
}
