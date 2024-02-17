using Anvil.API.Events;
using Anvil.API;
using NativeUtils = NWN.Systems.NativeUtils;
using NWN.Systems;
using NWN.Core;
using System.Linq;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void HandleTirDesOmbres(OnCreatureAttack onDamage)
    {
      int damage = onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18)
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      if (onDamage.Target is NwCreature target)
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(onDamage.Attacker) + onDamage.Attacker.GetAbilityModifier(Ability.Intelligence);
        int advantage = GetCreatureAbilityAdvantage(target, Ability.Wisdom);
        int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Wisdom, tirDC, advantage, feedback);
        bool saveFailed = totalSave < tirDC;

        SpellUtils.SendSavingThrowFeedbackMessage(onDamage.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Constitution);

        if (saveFailed)
          NWScript.AssignCommand(onDamage.Attacker, () => target.ApplyEffect(EffectDuration.Temporary,
            Effect.Blindness(), NwTimeSpan.FromRounds(1)));
      }

      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(damage, CustomDamageType.Psychic)));

      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir des Ombres", StringUtils.gold, true);
    }
  }
}
