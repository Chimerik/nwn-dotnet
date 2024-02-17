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
    public static void HandleTirBannissement(OnCreatureAttack onDamage)
    {
      if (onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level > 17))
        NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
          Effect.Damage(NwRandom.Roll(Utils.random, 6, 2), DamageType.Magical)));

      if (onDamage.Target is NwCreature target)
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(onDamage.Attacker) + onDamage.Attacker.GetAbilityModifier(Ability.Intelligence);
        int advantage = GetCreatureAbilityAdvantage(target, Ability.Charisma);
        int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Charisma, tirDC, advantage, feedback);
        bool saveFailed = totalSave < tirDC;

        SpellUtils.SendSavingThrowFeedbackMessage(onDamage.Attacker, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Constitution);

        if (saveFailed)
        {
          target.VisibilityOverride = Anvil.Services.VisibilityMode.Hidden;
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
          target.ApplyEffect(EffectDuration.Temporary, Effect.CutsceneParalyze(), NwTimeSpan.FromRounds(1));
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.tirBannissementEffect, NwTimeSpan.FromRounds(1));
        }
      }

      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir de Bannissement", StringUtils.gold, true);
    }
  }
}
