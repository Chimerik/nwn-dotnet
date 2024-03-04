using Anvil.API.Events;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamageCharmed(CreatureEvents.OnDamaged onDamage)
    {
      foreach (var eff in onDamage.Creature.ActiveEffects.Where(e => e.Tag == EffectSystem.CharmEffectTag && e.Creator == onDamage.Damager))
        onDamage.Creature.RemoveEffect(eff);
    }
  }
}
