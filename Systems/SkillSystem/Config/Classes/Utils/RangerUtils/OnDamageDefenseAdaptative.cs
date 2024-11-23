using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnDamageDefenseAdaptative(CreatureEvents.OnDamaged onDamaged)
    {
      var oDamager = NWScript.GetLastDamager(onDamaged.Creature).ToNwObject<NwObject>();

      if (oDamager is not NwCreature damager
        || damager.ActiveEffects.Any(e => e.Tag == EffectSystem.DefenseAdaptativeMalusEffectTag && e.Creator == onDamaged.Creature))
        return;

      NWScript.AssignCommand(onDamaged.Creature, () => damager.ApplyEffect(EffectDuration.Temporary, EffectSystem.DefenseAdaptativeMalus, TimeSpan.FromSeconds(5)));
    }
  }
}
