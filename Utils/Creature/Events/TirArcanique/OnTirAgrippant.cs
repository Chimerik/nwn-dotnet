using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using NWN.Core;
using System.Linq;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static async void HandleTirAgrippant(OnCreatureAttack onDamage)
    {
      int damage = onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18)
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
          Effect.Damage(damage, CustomDamageType.Poison)));

      onDamage.Target.GetObjectVariable<LocalVariableLocation>(TirAgrippantVariable).Value = onDamage.Target.Location;

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Temporary,
          EffectSystem.tirAgrippantEffect, NwTimeSpan.FromRounds(10)));

      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir Agrippant", StringUtils.gold, true);

      await NwTask.NextFrame();
      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
    }
  }
}
