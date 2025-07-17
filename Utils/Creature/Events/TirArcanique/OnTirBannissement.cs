using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void HandleTirBannissement(OnCreatureAttack onDamage)
    {
      if (onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level > 17))
        NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
          Effect.Damage(NwRandom.Roll(Utils.random, 6, 2), DamageType.Magical)));

      if (onDamage.Target is NwCreature target)
      {
        int tirDC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(onDamage.Attacker) + onDamage.Attacker.GetAbilityModifier(Ability.Intelligence);

        if (GetSavingThrowResult(target, Ability.Charisma, onDamage.Attacker, tirDC) == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetBannissementEffect(target), NwTimeSpan.FromRounds(1));
          onDamage.Attacker.ClearActionQueue();
        }
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir de Bannissement", StringUtils.gold, true);

      await NwTask.NextFrame();
      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
    }
  }
}
