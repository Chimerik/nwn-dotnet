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
        || onDamaged.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.DefenseAdaptativeEffectTag && e.Creator == damager))
        return;

      NWScript.AssignCommand(damager, () => onDamaged.Creature.ApplyEffect(EffectDuration.Temporary, EffectSystem.DefenseAdaptative, NwTimeSpan.FromRounds(1)));
    }
  }
}
