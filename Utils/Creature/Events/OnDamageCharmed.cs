using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using System.Linq;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnDamageCharmed(CreatureEvents.OnDamaged onDamage)
    {
      foreach (var eff in onDamage.Creature.ActiveEffects.Where(e => e.Tag == EffectSystem.charmEffectTag && e.Creator == onDamage.Damager))
        onDamage.Creature.RemoveEffect(eff);
    }
  }
}
