using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void HandleTirDesOmbres(OnCreatureAttack onDamage)
    {
      LogUtils.LogMessage($"-----------{onDamage.Attacker.Name} Tir des Ombres-------------", LogUtils.LogType.Combat);

      int damage = onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18)
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      if (onDamage.Target is NwCreature target)
      {
        int tirDC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onDamage.Attacker) + onDamage.Attacker.GetAbilityModifier(Ability.Intelligence);
        
        if (GetSavingThrowResult(target, Ability.Wisdom, onDamage.Attacker, tirDC) == SavingThrowResult.Failure)
          NWScript.AssignCommand(onDamage.Attacker, () => target.ApplyEffect(EffectDuration.Temporary,
            Effect.Blindness(), NwTimeSpan.FromRounds(1)));
      }

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(damage, CustomDamageType.Psychic)));

      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir des Ombres", StringUtils.gold, true);

      await NwTask.NextFrame();
      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
    }
  }
}
