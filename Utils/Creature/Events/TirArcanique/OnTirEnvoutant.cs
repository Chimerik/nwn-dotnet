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
    public static async void HandleTirEnvoutant(OnCreatureAttack onDamage)
    {
      int damage = onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18)
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(damage, CustomDamageType.Psychic)));

      if (onDamage.Target is NwCreature target)
      {
        bool saveFailed = false;

        if (!EffectSystem.IsCharmeImmune(target))
        {
          SpellConfig.SavingThrowFeedback feedback = new();
          int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(onDamage.Attacker) + onDamage.Attacker.GetAbilityModifier(Ability.Intelligence);
          int advantage = GetCreatureAbilityAdvantage(target, Ability.Wisdom, effectType: SpellConfig.SpellEffectType.Charm);
          int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Wisdom, tirDC, advantage, feedback);
          saveFailed = totalSave < tirDC;

          SpellUtils.SendSavingThrowFeedbackMessage(onDamage.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Constitution);
        }
        else
          onDamage.Attacker.LoginPlayer?.SendServerMessage($"{StringUtils.ToWhitecolor(target.Name)} est immunisé à votre effet de charme", ColorConstants.Orange);

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

      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir Envoûtant", StringUtils.gold, true);

      await NwTask.NextFrame();
      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
    }
  }
}
