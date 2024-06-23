using System.Linq;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static void OnDamageAbjurationWard(CreatureEvents.OnDamaged onDamage)
    {
      var ward = onDamage.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.AbjurationWardEffectTag);

      if(ward is null)
      {
        onDamage.Creature.OnDamaged -= OnDamageAbjurationWard;
        return;
      }

      EffectSystem.AbjurationWardIntensityReduction(ward, onDamage.Creature);
    }
  }
}
