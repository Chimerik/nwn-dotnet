using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static void OnDamageElectrocution(OnCreatureDamage onDamage)
    {
      if (onDamage.DamagedBy is not NwCreature caster || onDamage.Target is not NwCreature target || target.Size > CreatureSize.Large 
        || (onDamage.DamageData.GetDamageByType(DamageType.Sonic) < 1 && onDamage.DamageData.GetDamageByType(DamageType.Electrical) < 1))
        return;

      EffectSystem.ApplyKnockdown(target, caster, Ability.Wisdom, Ability.Constitution, EffectSystem.Destabilisation);
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSonic));
    }
  }
}
