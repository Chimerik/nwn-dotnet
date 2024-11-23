using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnDamageDefenseSuperieure(CreatureEvents.OnDamaged onDamaged)
    {
      var damaged = onDamaged.Creature;
      List<KeyValuePair<DamageType, int>> damageList = new();
      
      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
        damageList.Add(KeyValuePair.Create(damageType, onDamaged.GetDamageDealtByType(damageType)));

      if (damageList.Count < 1)
        return;

      damageList.Sort((x, y) => y.Value.CompareTo(x.Value));
      damaged.ApplyEffect(EffectDuration.Temporary, Effect.DamageImmunityIncrease(damageList.FirstOrDefault().Key, 50), NwTimeSpan.FromRounds(1));
    }
  }
}
