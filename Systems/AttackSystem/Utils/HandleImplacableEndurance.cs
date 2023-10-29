using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class DamageUtils
  {
    public static void HandleImplacableEndurance(OnCreatureDamage onDamage, NwCreature target)
    {
      if (!target.ActiveEffects.Any(e => e.Tag == EffectSystem.EnduranceImplacableEffectTag))
        return;

      int totalDamage = 0;

      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
        totalDamage += onDamage.DamageData.GetDamageByType(damageType) > -1 
          ? onDamage.DamageData.GetDamageByType(damageType) : 0;

      if(target.HP <= totalDamage)
      {
        target.ApplyEffect(EffectDuration.Temporary, Effect.TemporaryHitpoints(totalDamage - target.HP + 1), TimeSpan.FromSeconds(6));
          
        foreach(var eff in target.ActiveEffects)
          if(eff.Tag == EffectSystem.EnduranceImplacableEffectTag)
            target.RemoveEffect(eff);

        target.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Delete();
      }
    }
  }
}
