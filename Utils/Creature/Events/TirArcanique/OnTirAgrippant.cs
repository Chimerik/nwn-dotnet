using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using NWN.Core;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void HandleTirAgrippant(OnCreatureAttack onDamage)
    {
      int damage = onDamage.Attacker.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter))?.Level < 18 
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
          Effect.Damage(damage, CustomDamageType.Poison)));

      onDamage.Target.GetObjectVariable<LocalVariableLocation>(TirAgrippantVariable).Value = onDamage.Target.Location;

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Temporary,
          EffectSystem.tirAgrippantEffect, NwTimeSpan.FromRounds(10)));

      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir Agrippant", StringUtils.gold, true);
    }
  }
}
