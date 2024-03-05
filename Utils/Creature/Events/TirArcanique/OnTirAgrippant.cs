using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void HandleTirAgrippant(OnCreatureAttack onDamage)
    {
      bool archerLevel18 = onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level > 17);
      int damage = archerLevel18 ? NwRandom.Roll(Utils.random, 6, 4) : NwRandom.Roll(Utils.random, 6, 2);

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
          Effect.Damage(damage, CustomDamageType.Poison)));

      LogUtils.LogMessage($"Tir Agrippant : +{(archerLevel18 ? 4 : 2)}d6 (poison) => +{damage}", LogUtils.LogType.Combat);

      onDamage.Target.GetObjectVariable<LocalVariableLocation>(TirAgrippantVariable).Value = onDamage.Target.Location;

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Temporary,
          EffectSystem.tirAgrippantEffect, NwTimeSpan.FromRounds(10)));

      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir Agrippant", StringUtils.gold, true);

      await NwTask.NextFrame();
      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
    }
  }
}
