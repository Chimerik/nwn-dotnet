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
        int tirDC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onDamage.Attacker) + onDamage.Attacker.GetAbilityModifier(Ability.Intelligence);

        if (GetSavingThrowResult(target, Ability.Constitution, onDamage.Attacker, tirDC) == SavingThrowResult.Failure)
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
