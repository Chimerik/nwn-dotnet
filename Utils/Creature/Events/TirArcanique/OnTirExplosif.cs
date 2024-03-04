using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void HandleTirExplosif(OnCreatureAttack onDamage)
    {
      int damage = onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18)
        ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
        Effect.VisualEffect(VfxType.FnfGasExplosionFire)));

      foreach(var creature in onDamage.Target.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 3, false))
      {
        NWScript.AssignCommand(onDamage.Attacker, () => creature.ApplyEffect(EffectDuration.Instant,
          Effect.Damage(damage, DamageType.Magical)));
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir Explosif", StringUtils.gold, true);

      await NwTask.NextFrame();
      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
    }
  }
}
