using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static void OnDamageBenedictionDuMalin(OnCreatureDamage onDamage)
    {
      if (onDamage.DamagedBy is not NwCreature caster || caster.HP < 1 || onDamage.Target is not NwCreature target || target.HP < 1)
        return;

      int totalDamage = 0;

      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
        if (onDamage.DamageData.GetDamageByType(damageType) > 0)
          totalDamage += onDamage.DamageData.GetDamageByType(damageType);

      if (target.HP - totalDamage > 0)
        return;
      
      int chaMod = caster.GetAbilityModifier(Ability.Charisma) > 1 ? caster.GetAbilityModifier(Ability.Charisma) : 1;
      chaMod += caster.GetClassInfo((ClassType)CustomClass.Occultiste).Level;

      caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(chaMod));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));

      EffectUtils.RemoveTaggedEffect(target, caster, EffectSystem.BenedictionDuMalinEffectTag);
    }
  }
}
