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
      LogUtils.LogMessage($"--- {onDamage.Attacker.Name} Tir Explosif ---", LogUtils.LogType.Combat);

      int nbDice = onDamage.Attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18) ? 2 : 4;

      NWScript.AssignCommand(onDamage.Attacker, () => onDamage.Target.ApplyEffect(EffectDuration.Instant,
        Effect.VisualEffect(VfxType.FnfGasExplosionFire)));

      foreach(var creature in onDamage.Target.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 3, false))
      {
        int damage = NwRandom.Roll(Utils.random, 6, nbDice);

        NWScript.AssignCommand(onDamage.Attacker, () => creature.ApplyEffect(EffectDuration.Instant,
          Effect.Damage(damage, DamageType.Magical)));

        LogUtils.LogMessage($"{creature.Name} : {nbDice}d6 = {damage}", LogUtils.LogType.Combat);
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Attacker, "Tir Explosif", StringUtils.gold, true);
      LogUtils.LogMessage($"------", LogUtils.LogType.Combat);

      await NwTask.NextFrame();
      onDamage.Attacker.OnCreatureAttack -= OnAttackTirArcanique;
    }
  }
}
