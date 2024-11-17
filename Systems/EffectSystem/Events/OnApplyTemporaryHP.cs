using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnApplyTemporaryHP(OnEffectApply onEffect)
    {
      if (onEffect.Effect.EffectType != EffectType.TemporaryHitpoints || onEffect.Object is not NwCreature creature)
        return;

      var newEffect = onEffect.Effect;
      var existingEffect = creature.ActiveEffects.FirstOrDefault(e => e.EffectType == EffectType.TemporaryHitpoints);

      if (existingEffect is null)
        return;

      if (newEffect.IntParams[0] > existingEffect.IntParams[0])
        EffectUtils.RemoveEffectType(creature, EffectType.TemporaryHitpoints);
      else
        onEffect.PreventApply = true;
    }
  }
}
