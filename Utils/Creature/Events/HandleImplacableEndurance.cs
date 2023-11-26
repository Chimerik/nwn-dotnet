using System;
using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void HandleImplacableEndurance(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Creature.HP <= onDamage.DamageAmount)
      {
        onDamage.Creature.ApplyEffect(EffectDuration.Temporary, Effect.TemporaryHitpoints(onDamage.DamageAmount - onDamage.Creature.HP + 1), TimeSpan.FromSeconds(6));

        foreach (var eff in onDamage.Creature.ActiveEffects)
          if (eff.Tag == EffectSystem.EnduranceImplacableEffectTag)
            onDamage.Creature.RemoveEffect(eff);

        onDamage.Creature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Delete();
        onDamage.Creature.OnDamaged -= HandleImplacableEndurance;
      }
    }
  }
}
