﻿using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void HandleTirExplosif(OnCreatureAttack onDamage)
    {
      int damage = onDamage.Attacker.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter))?.Level < 18
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
        Effect.VisualEffect(VfxType.FnfGasExplosionFire)));

      foreach(var creature in onDamage.Target.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 3, false))
      {
        NWScript.AssignCommand(onDamage.Attacker, () => creature.ApplyEffect(EffectDuration.Instant,
          Effect.Damage(damage, DamageType.Magical)));
      }

      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir Explosif", StringUtils.gold, true);
    }
  }
}
