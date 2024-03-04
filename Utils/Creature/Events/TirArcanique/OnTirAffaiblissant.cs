using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void HandleTirAffaiblissant(OnCreatureAttack onDamage)
    {
      int damage = onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18)
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      if (onDamage.Target is NwCreature target)
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(onDamage.Attacker) + onDamage.Attacker.GetAbilityModifier(Ability.Intelligence);
        int advantage = GetCreatureAbilityAdvantage(target, Ability.Constitution);
        int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Constitution, tirDC, advantage, feedback);
        bool saveFailed = totalSave < tirDC;

        SpellUtils.SendSavingThrowFeedbackMessage(onDamage.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Constitution);

        if (saveFailed)
        {
          target.GetObjectVariable<LocalVariableInt>(TirAffaiblissantVariable).Value = 1;

          NWScript.AssignCommand(onDamage.Attacker, () => target.ApplyEffect(EffectDuration.Temporary,
          EffectSystem.tirAffaiblissantEffect, NwTimeSpan.FromRounds(1)));
        }
      }

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(damage, CustomDamageType.Necrotic)));

      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir Affaiblissant", StringUtils.gold, true);
      LogUtils.LogMessage($"Tir affaiblissant - Dégâts : {damage}", LogUtils.LogType.Combat);

      await NwTask.NextFrame();
      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
    }
  }
}
