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

      switch(damageList.FirstOrDefault().Key)
      {
        case DamageType.Acid: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceAcide, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Fire: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceFeu, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Sonic: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceTonnerre, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Cold: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceFroid, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Magical: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceForce, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Electrical: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceElec, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Divine: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceRadiant, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Bludgeoning: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceContondant, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Slashing: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceTranchant, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Piercing: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistancePercant, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Custom1: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistancePoison, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Custom2: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistanceNecrotique, NwTimeSpan.FromRounds(1)); break;
        case DamageType.Custom3: damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResistancePsychique, NwTimeSpan.FromRounds(1)); break;
      }
    }
  }
}
